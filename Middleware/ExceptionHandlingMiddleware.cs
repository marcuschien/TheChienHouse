using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TheChienHouse.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";
            var status = StatusCodes.Status500InternalServerError;
            string title = "An unexpected error occurred.";

            if (exception is TheChienHouse.Exceptions.NotFoundException)
            {
                status = StatusCodes.Status404NotFound;
                title = exception.Message ?? "Resource not found.";
            }
            else if (exception is ArgumentException || exception is InvalidOperationException)
            {
                status = StatusCodes.Status400BadRequest;
                title = exception.Message ?? "Bad request.";
            }

            var problem = new
            {
                type = $"https://httpstatuses.com/{status}",
                title,
                status,
                detail = exception.Message
            };

            context.Response.StatusCode = status;
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
        }
    }
}