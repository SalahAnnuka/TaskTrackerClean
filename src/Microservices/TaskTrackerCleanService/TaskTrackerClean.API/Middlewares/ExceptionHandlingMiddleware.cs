using Common.Contracts.Dtos;
using Common.Contracts.Encryption;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;
using System.Text.Json;

namespace TaskTrackerClean.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly EncryptionHelper _encryptionHelper;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ProblemDetailsFactory problemDetailsFactory,
            ILogger<ExceptionHandlingMiddleware> logger,
            EncryptionHelper helper
            )
        {
            _next = next;
            _problemDetailsFactory = problemDetailsFactory;
            _logger = logger;
            _encryptionHelper = helper;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred. \ntraceId: {context.TraceIdentifier} \n");

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

                    var serializedMessage = JsonSerializer.Serialize(logMessage);

                    var encryptedString = _encryptionHelper.Encrypt(serializedMessage);
                    var encryptedMessage = new EncryptedMessage
                    {
                        Service = "TaskTrackerService",
                        TraceId = context.TraceIdentifier,
                        EncryptedString = encryptedString
                    };

                    await publishEndpoint.Publish(encryptedMessage, context.RequestAborted);
                    Console.WriteLine("\nLog has been published successfully.\n\n");


                }

                catch (Exception publishEx)
                {
                    _logger.LogError(publishEx, "Failed to publish exception log message\n\n");
                }


                var json = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
