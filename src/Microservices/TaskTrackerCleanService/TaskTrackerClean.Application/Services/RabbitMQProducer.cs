using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TaskTrackerClean.Application.Services
{
    public class RabbitMQProducer : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection = null!;
        private IChannel _channel = null!;

        public RabbitMQProducer()
        {
            _factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://loggeruser:12345678@localhost:5672/loggerhost"),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                ClientProvidedName = "TaskTrackerProducer"
            };
        }

        private async Task EnsureConnectedAsync()
        {
            if (_connection?.IsOpen == true && _channel?.IsOpen == true)
                return;

            _channel?.Dispose();
            _connection?.Dispose();

            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                exchange: "logs",
                type: ExchangeType.Fanout,
                durable: true);

            await _channel.QueueDeclareAsync(
                queue: "exception-logs-queue",
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: "exception-logs-queue",
                exchange: "logs",
                routingKey: "");
        }

        public async Task PublishLog(object body, string routingKey = "")
        {
            await EnsureConnectedAsync();

            var json = JsonSerializer.Serialize(body);
            var bodyBytes = Encoding.UTF8.GetBytes(json);

            Console.WriteLine($"Publishing to exchange: logs, routingKey: {routingKey}");
            Console.WriteLine($"Message: {json}");
            Console.WriteLine($"Connection is open: {_connection?.IsOpen}");
            Console.WriteLine($"Channel is open: {_channel?.IsOpen}");

            if (_channel != null)
                await _channel.BasicPublishAsync(
                    exchange: "logs",
                    routingKey: routingKey,
                    body: bodyBytes,
                    mandatory: false);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}