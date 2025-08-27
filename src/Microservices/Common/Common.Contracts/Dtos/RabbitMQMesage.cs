namespace Common.Contracts.Dtos
{
    public class RabbitMQMesage
    {
        public DateTime? Timestamp { get; set; }
        public string? Service { get; set; }
        public string? TraceId { get; set; }

    }
}
