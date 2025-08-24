// RabbitMQConsumerService.cs
using Logger.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using static MongoDB.Driver.WriteConcern;

namespace Logger.API.Services
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ExceptionLoggerService _loggerService;
        private readonly ILogger<RabbitMQConsumerService> _logger;

        public RabbitMQConsumerService(
            IConfiguration configuration,
            ExceptionLoggerService loggerService,
            ILogger<RabbitMQConsumerService> logger)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _loggerService = loggerService;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel.QueueDeclare(
                queue: "exception_logs",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var logEntry = JsonSerializer.Deserialize<ExceptionLogEntity>(message);

                    await _loggerService.LogExceptionAsync(
                        logEntry.Level,
                        logEntry.Message,
                        logEntry.Exception,
                        logEntry.TraceId
                    );

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from RabbitMQ");
                }
            };

            _channel.BasicConsume(
                queue: "exception_logs",
                autoAck: false,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}