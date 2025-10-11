// ============= HI, Updated by Mousa ======================
// This file contains extension methods to register and configure all services,
// repositories, middleware, and technical features in ASP.NET Core Dependency Injection.
// =============== Updated on 11 Oct 2025 ====================


using Asp.Versioning;
using Audiophile.Application.Services;
using Audiophile.Application.Services.AuthServices;
using Audiophile.Domain.Interfaces;
using Audiophile.Infrastructure.Data;
using Audiophile.Infrastructure.Repositories;
using AudiophileEcommerceAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

namespace AudiophileEcommerceAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAudiophileServices(this IServiceCollection services, IConfiguration config)
        {
            // Logging (Serilog può essere configurato in Program.cs se serve pipeline custom)
            // DbContext - PostgreSQL
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
                    config.GetConnectionString("DefaultConnection"),
                    npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null
                    )
                );
            });

            // DI dei Repository
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICartRepository, CartRepository>();

            // DI dei Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // AutoMapper
            //services.AddAutoMapper(typeof(Program));
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            // Rate Limiting
            services.AddRateLimiter(options =>
            {
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
                options.AddFixedWindowLimiter("auth", opts =>
                {
                    opts.PermitLimit = 5;
                    opts.Window = TimeSpan.FromMinutes(1);
                    opts.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opts.QueueLimit = 0;
                });
                options.AddSlidingWindowLimiter("api", opts =>
                {
                    opts.PermitLimit = 50;
                    opts.Window = TimeSpan.FromMinutes(1);
                    opts.SegmentsPerWindow = 4;
                    opts.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opts.QueueLimit = 0;
                });
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = "Troppi tentativi",
                        message = "Hai superato il limite di richieste. Riprova tra qualche minuto.",
                        retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter) ? retryAfter.TotalSeconds : 60
                    }, cancellationToken: cancellationToken);
                };
            });

            // Memory & Redis Cache
            services.AddMemoryCache();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetConnectionString("Redis");
                options.InstanceName = "Audiophile_";
            });

            // Health Checks
            services.AddHealthChecks()
                .AddDbContextCheck<AppDbContext>("database")
                .AddRedis(config.GetConnectionString("Redis") ?? "localhost:6379", "redis");

            // API Versioning
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            // Authentication & Authorization
            var jwtSettings = config.GetSection("JwtSettings");
            var secretKey = config["JwtSettings:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 64)
                throw new InvalidOperationException("JWT Secret deve essere almeno 64 caratteri (512 bit)");

            services.AddAuthentication(options =>
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

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
            });

            // CORS Policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                        config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" }
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            // Response Compression
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
            });

            // Controllers & Filters
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

            return services;
        }
    }

}
