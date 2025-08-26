using Logger.Application.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Logger.Application.Services
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection = null!;
        private IChannel _channel = null!;
        private readonly ILogger<RabbitMQConsumerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private const string ExchangeName = "logs";
        private const string QueueName = "exception-logs-queue";

        public RabbitMQConsumerService(
            ILogger<RabbitMQConsumerService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            _factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://loggeruser:12345678@localhost:5672/loggerhost"),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                ClientProvidedName = "LoggerConsumer"
            };
        }

        private async Task EnsureConnectedAsync()
        {
            if (_connection?.IsOpen == true && _channel?.IsOpen == true)
                return;

            // Clean up old resources if needed
            _channel?.Dispose();
            _connection?.Dispose();

            // Create new connection and channel
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            // Declare exchange
            await _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Fanout,
                durable: true);

            // Declare and bind queue
            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: "");

            _logger.LogInformation("RabbitMQ Consumer connection established successfully.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await EnsureConnectedAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    _logger.LogDebug("Received message: {Message}", message);

                    // Deserialize the log entry
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };

                    var logEntry = JsonSerializer.Deserialize<ExceptionLogDto>(message, jsonOptions);

                    if (logEntry == null)
                    {
                        _logger.LogWarning("Failed to deserialize log entry: {Message}", message);
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                        return;
                    }

                    // Process the log entry using scoped service
                    using var scope = _serviceProvider.CreateScope();
                    var loggerService = scope.ServiceProvider.GetRequiredService<ExceptionLoggerService>();

                    await loggerService.LogExceptionAsync(logEntry);

                    // Acknowledge the message
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    _logger.LogDebug("Successfully processed log entry with TraceId: {TraceId}", logEntry.TraceId);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Failed to deserialize message: {Message}", message);
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process message: {Message}", message);
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("RabbitMQ Consumer started listening on queue: {QueueName}", QueueName);

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override void Dispose()
        {
            try
            {
                _channel?.Dispose();
                _connection?.Dispose();
                _logger.LogInformation("RabbitMQ Consumer connection disposed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while disposing RabbitMQ Consumer connection.");
            }

            base.Dispose();
        }
    }
}