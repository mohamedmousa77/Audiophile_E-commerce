using Audiophile.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AudiophileEcommerceAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAudiophileServices(this IServiceCollection services, IConfiguration config)
        {
            // ===== SERILOG CONFIGURATION =====
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console()
                .WriteTo.File("logs/audiophile-.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Seq(config["Seq:ServerUrl"] ?? "http://localhost:5341")
                .CreateLogger();

            builder.Host.UseSerilog();

            // DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            });

            // Caching
            services.AddMemoryCache();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetConnectionString("Redis");
                options.InstanceName = "Audiophile_";
            });

            // Rate Limiter, Versioning, JWT...
            // Tutta la configurazione dei servizi aggiuntivi

            // DI dei repository, services, automapper
            // services.AddScoped<IAuthRepository, AuthRepository>();
            // services.AddAutoMapper(typeof(Program));

            return services;
        }
    }

}
