// ============= HI, Updated by Mousa ========================
// This file defines extension methods that neatly install all the ASP.NET Core pipeline middleware,
// separating exception handling, authentication, rate limiting, logging, CORS, routing, and compression.
// =============== Updated on 11 Oct 2025 ====================


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
            app.UseResponseCompression();
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
                    diagnosticContext.Set("UserName", httpContext.User.Identity?.Name ?? "Anonymous");
                };
            });
            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
