using DnTech.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace DnTech.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió una excepción no controlada: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Determinar código de estado basado en el tipo de excepción
            // Nota: El orden importa - de más específico a más genérico
            var exceptionTypeName = exception.GetType().Name;

            var statusCode = exceptionTypeName switch
            {
                "UserNotFoundException" => HttpStatusCode.NotFound,
                "InvalidCredentialsException" => HttpStatusCode.Unauthorized,
                "DuplicateEmailException" => HttpStatusCode.Conflict,
                "UnauthorizedAccessException" => HttpStatusCode.Unauthorized,
                _ when exception.GetType().BaseType?.Name == "DomainException" => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            var response = new
            {
                statusCode = (int)statusCode,
                message = exception.Message,
                type = exception.GetType().Name,
                timestamp = DateTime.UtcNow
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
