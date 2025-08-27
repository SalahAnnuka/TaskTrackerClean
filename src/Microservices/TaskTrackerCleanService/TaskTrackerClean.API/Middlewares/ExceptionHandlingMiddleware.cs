using Common.Contracts.Dtos;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Transports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace TaskTrackerClean.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ProblemDetailsFactory problemDetailsFactory,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _problemDetailsFactory = problemDetailsFactory;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred. \ntraceId: {context.TraceIdentifier} \n\n");

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

                try
                {
                    var publishEndpoint = context.RequestServices.GetRequiredService<IPublishEndpoint>();

                    var logMessage = new ExceptionLogMessage
                    {
                        Message = ex.Message,
                        Service = "TaskTrackerService",
                        TraceId = context.TraceIdentifier,
                        Level = "Error",
                        Exception = ex.ToString(),
                        Timestamp = DateTime.UtcNow
                    };

                    await publishEndpoint.Publish(logMessage, context.RequestAborted);
                    Console.WriteLine("\n\nLog has been published successfully.");


                }

                catch (Exception publishEx)
                {
                    _logger.LogError(publishEx, "Failed to publish exception log message");
                }


                var json = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
