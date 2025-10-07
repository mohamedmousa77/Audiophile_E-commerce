using Audiophile.Application.Services;
using Audiophile.Application.Services.AuthServices;
using Audiophile.Domain.Interfaces;
using Audiophile.Infrastructure.Data;
using Audiophile.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ====================================
//          DATABASE Config
// ====================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ====================================
//      Repositories & Services (DI)
// ====================================

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<CartService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();

// ====================================
//      JWT Authentication
// ====================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey mancante");

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});
// ====================================
//          Rate Limiting (Security)
// ====================================
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Troppi tentativi. Riprova tra 1 minuto."
        }, cancellationToken: token);
    };
});


// Configuration base:

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  CORS POLICY
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseCors("AllowFrontend");

// SEED iniziale
//using (var scope = app.Services.CreateScope())
//{
//    var seeder = scope.ServiceProvider.GetRequiredService<ISeedDataService>();
//    seeder.Seed();
//}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        config.RoutePrefix = string.Empty;
    });
}



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();
