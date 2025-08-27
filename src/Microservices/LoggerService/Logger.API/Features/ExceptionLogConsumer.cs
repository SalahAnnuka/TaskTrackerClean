using Common.Contracts.Dtos;
using Logger.Application.Services;
using MassTransit;

namespace Logger.API.Features
{
    public class ExceptionLogConsumer : IConsumer<ExceptionLogMessage>
    {
        private readonly ExceptionLoggerService _exceptionLoggerService;

        public ExceptionLogConsumer(ExceptionLoggerService exceptionLoggerService)
        {
            _exceptionLoggerService = exceptionLoggerService;
        }

        public async Task Consume(ConsumeContext<ExceptionLogMessage> context)
        {
            var message = context.Message;

            await _exceptionLoggerService.LogExceptionAsync(message);

        }
    }
}
