namespace TaskTrackerClean.API.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using System.Net;
    using System.Text.Json;
    using TaskTrackerClean.API.Services;

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IRabbitMQProducer _rabbitMQProducer;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ProblemDetailsFactory problemDetailsFactory,
            ILogger<ExceptionHandlingMiddleware> logger,
            IRabbitMQProducer rabbitMQProducer)
        {
            _next = next;
            _problemDetailsFactory = problemDetailsFactory;
            _logger = logger;
            _rabbitMQProducer = rabbitMQProducer;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred. \ntraceId: {context.TraceIdentifier}");

                var logEntry = new
                {
                    Level = "Error",
                    Message = ex.Message,
                    Exception = ex.ToString(),
                    TraceId = context.TraceIdentifier,
                    Timestamp = DateTime.UtcNow
                };

                _rabbitMQProducer.PublishLogMessage(logEntry, "exception_logs");

                var statusCode = ex switch
                {
                    KeyNotFoundException => HttpStatusCode.NotFound,
                    ArgumentException => HttpStatusCode.BadRequest,
                    InvalidOperationException => HttpStatusCode.BadRequest,
                    _ => HttpStatusCode.InternalServerError
                };

                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    context,
                    statusCode: (int)statusCode,
                    title: "An error occurred while processing your request.",
                    detail: ex.Message,
                    instance: context.Request.Path
                );

                problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/problem+json";

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(problemDetails, options);
                await context.Response.WriteAsync(json);
            }
        }
    }

}
