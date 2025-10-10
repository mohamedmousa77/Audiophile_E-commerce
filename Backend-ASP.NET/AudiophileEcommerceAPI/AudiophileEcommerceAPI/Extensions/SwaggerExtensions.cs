namespace AudiophileEcommerceAPI.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddAudiophileSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Audiophile E-Commerce API",
                    Version = "v1",
                    Description = "API completa per gestione e-commerce audio",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Mohamed Mousa",
                        Email = "mohamed@audiophile.com",
                        Url = new Uri("https://github.com/mohamedmousa77")
                    }
                });
                // JWT Bearer configuration, XML comments, etc.
            });
            return services;
        }
    }
}
