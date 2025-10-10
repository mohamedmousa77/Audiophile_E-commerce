using AudiophileEcommerceAPI.Middleware;
using Serilog;

namespace AudiophileEcommerceAPI.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseAudiophilePipeline(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCompression();
            app.UseSerilogRequestLogging();
            return app;
        }
    }
}
