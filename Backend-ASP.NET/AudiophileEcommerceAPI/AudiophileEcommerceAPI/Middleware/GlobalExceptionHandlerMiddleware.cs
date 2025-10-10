using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace AudiophileEcommerceAPI.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);

            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                TraceId = context.TraceIdentifier,
                Instance = context.Request.Path,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case ArgumentException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Status = 400;
                    errorResponse.Title = "Richiesta non valida";
                    errorResponse.Detail = argEx.Message;
                    _logger.LogWarning(argEx, "Bad request: {Message}", argEx.Message);
                    break;

                case UnauthorizedAccessException unauthEx:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Status = 401;
                    errorResponse.Title = "Non autorizzato";
                    errorResponse.Detail = "Devi effettuare il login per accedere a questa risorsa";
                    _logger.LogWarning(unauthEx, "Unauthorized access attempt");
                    break;

                case KeyNotFoundException notFoundEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Status = 404;
                    errorResponse.Title = "Risorsa non trovata";
                    errorResponse.Detail = notFoundEx.Message;
                    _logger.LogWarning(notFoundEx, "Resource not found: {Message}", notFoundEx.Message);
                    break;

                case InvalidOperationException invOpEx:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse.Status = 409;
                    errorResponse.Title = "Operazione non valida";
                    errorResponse.Detail = invOpEx.Message;
                    _logger.LogWarning(invOpEx, "Invalid operation: {Message}", invOpEx.Message);
                    break;

                case DbUpdateException dbEx:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Status = 500;
                    errorResponse.Title = "Errore database";
                    errorResponse.Detail = "Si è verificato un errore durante il salvataggio dei dati";
                    _logger.LogError(dbEx, "Database error: {Message}", dbEx.Message);
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Status = 500;
                    errorResponse.Title = "Errore interno del server";
                    errorResponse.Detail = "Si è verificato un errore imprevisto. Riprova più tardi.";
                    _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                    break;
            }

            // In development, includi stack trace
            if (_env.IsDevelopment())
            {
                errorResponse.DeveloperMessage = exception.ToString();
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(jsonResponse);
        }
    }

    public class ErrorResponse
    {
        public int Status { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? DeveloperMessage { get; set; }
    }

}
