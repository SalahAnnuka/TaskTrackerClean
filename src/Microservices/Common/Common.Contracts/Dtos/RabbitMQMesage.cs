using MassTransit;

namespace Common.Contracts.Dtos
{
    [ExcludeFromTopology]
    public class RabbitMQMesage
    {
        public DateTime? Timestamp { get; set; }
        public string? Service { get; set; }
        public string? TraceId { get; set; }

    }
}
