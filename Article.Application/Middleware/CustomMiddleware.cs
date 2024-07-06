using Article.Application.CustomExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Article.Application.Middleware
{
    public class CustomMiddleware : IMiddleware
    {
        private readonly ILogger<CustomMiddleware> _logger;

        public CustomMiddleware(ILogger<CustomMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
                _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
                Console.WriteLine($"Response: {context.Response.StatusCode}");
                _logger.LogInformation($"Response: {context.Response.StatusCode}");
                await next.Invoke(context);
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                _logger.LogError(ex.Message);

                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Data Not Found.",
                    Detailed = ex.Message // Optional: you can include more details or remove this line in production
                };

                var jsonResponse = JsonSerializer.Serialize(response);
                context.Response.WriteAsync(jsonResponse);
                return;
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                _logger.LogError(ex.Message);

                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Internal Server Error. An unexpected error occurred.",
                    Detailed = ex.Message // Optional: you can include more details or remove this line in production
                };

                var jsonResponse = JsonSerializer.Serialize(response);
                context.Response.WriteAsync(jsonResponse);
                return;
            }
        }
    }
}
