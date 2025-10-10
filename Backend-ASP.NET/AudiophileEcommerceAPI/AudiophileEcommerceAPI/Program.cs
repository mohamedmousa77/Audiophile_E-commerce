using Asp.Versioning;
using Audiophile.Application.Services;
using Audiophile.Application.Services.AuthServices;
using Audiophile.Domain.Interfaces;
using Audiophile.Infrastructure.Data;
using Audiophile.Infrastructure.Repositories;
using AudiophileEcommerceAPI.Data;
using AudiophileEcommerceAPI.Extensions;
using AudiophileEcommerceAPI.Filters;
using AudiophileEcommerceAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);



// ===== SERILOG CONFIGURATION =====
builder.Services.AddAudiophileServices(builder.Configuration);
builder.Services.AddAudiophileSwagger();


builder.Host.UseSerilog();


// ====================================
//          DATABASE Config
// ====================================

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null
            );
        });
});


// ===== REPOSITORIES =====
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// ===== SERVICES =====
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// ===== AUTO MAPPER ===== 
builder.Services.AddAutoMapper(typeof(Program));

// ====================================
//      JWT Authentication
// ====================================

// ===== JWT AUTHENTICATION =====
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = builder.Configuration["JwtSettings:Secret"]
    ?? throw new InvalidOperationException("JWT Secret non configurato");

if (secretKey.Length < 64) // 512 bit minimum
{
    throw new InvalidOperationException("JWT Secret deve essere almeno 64 caratteri (512 bit)");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning("JWT Authentication failed: {Message}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Log.Information("JWT Token validated for user: {User}",
                context.Principal?.Identity?.Name ?? "Unknown");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});


// ====================================
//          Rate Limiting (Security)
// ====================================

builder.Services.AddRateLimiter(options =>
{
    // Global rate limit
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var userId = context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });

    // Policy specifica per autenticazione (più restrittiva)
    options.AddFixedWindowLimiter("auth", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    });

    // Policy per API generali
    options.AddSlidingWindowLimiter("api", options =>
    {
        options.PermitLimit = 50;
        options.Window = TimeSpan.FromMinutes(1);
        options.SegmentsPerWindow = 4;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        Log.Warning("Rate limit exceeded for {IP} on {Path}",
            context.HttpContext.Connection.RemoteIpAddress,
            context.HttpContext.Request.Path);

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Troppi tentativi",
            message = "Hai superato il limite di richieste. Riprova tra qualche minuto.",
            retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                ? retryAfter.TotalSeconds
                : 60
        }, cancellationToken: cancellationToken);
    };
});


// Configuration base

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//  CORS POLICY
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:4200" }
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();

        //policy.WithOrigins("http://localhost:4200") // Angular dev server
        //      .AllowAnyHeader()
        //      .AllowAnyMethod();
    });
});

// ===== CACHING =====
builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Audiophile_";
});

// ===== HEALTH CHECKS =====
builder.Services.AddHealthChecks()
                .AddDbContextCheck<AppDbContext>("database")
                .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379", "redis");

// ===== API VERSIONING =====
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// ===== RESPONSE COMPRESSION =====
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

// ===== CONTROLLERS =====
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>(); // Custom validation filter
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// ===== SWAGGER =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Audiophile E-Commerce API",
        Version = "v1",
        Description = "API completa per gestione e-commerce audio",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Mohamed Mousa",
            Email = "mohamedellitheey@gmail.com",
            Url = new Uri("https://github.com/mohamedmousa77")
        }
    });

    // JWT Bearer configuration
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});


// ==========================
//      Build the app
// ==========================
var app = builder.Build();

// Global exception handler
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// HSTS and HTTPS
if (app.Environment.IsProduction())
{
    app.UseHsts(); // HTTP Strict Transport Security
}

app.UseHttpsRedirection();

// Swagger
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Audiophile API v1");
        options.RoutePrefix = string.Empty; // Swagger at root
    });
}

// Response compression
app.UseResponseCompression();

// Routing
app.UseRouting();

// CORS
app.UseCors("AllowFrontend");

// Rate limiting
app.UseRateLimiter();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Request logging
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
        diagnosticContext.Set("UserName", httpContext.User.Identity?.Name ?? "Anonymous");
    };
});

// Controllers
app.MapControllers();

// Health checks
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = entry.Value.Duration.TotalMilliseconds,
                description = entry.Value.Description,
                exception = entry.Value.Exception?.Message
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        });
        await context.Response.WriteAsync(result);
    }
});

// Database migration e seeding all’avvio (solo in sviluppo)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();

    // Seed data
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData.Initialize(context);
}

try
{
    Log.Information("Starting Audiophile E-Commerce API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}




//app.UseCors("AllowFrontend");

// SEED iniziale
//using (var scope = app.Services.CreateScope())
//{
//    var seeder = scope.ServiceProvider.GetRequiredService<ISeedDataService>();
//    seeder.Seed();
//}

// Middleware pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(config =>
//    {
//        config.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
//        config.RoutePrefix = string.Empty;
//    });
//}



//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();
//app.UseRateLimiter();
//app.MapControllers();

//app.Run();
