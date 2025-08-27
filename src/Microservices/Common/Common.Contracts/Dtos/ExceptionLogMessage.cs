namespace Common.Contracts.Dtos
{
    public class ExceptionLogMessage : RabbitMQMesage
    {
        public string? Level { get; set; }

        public string? Message { get; set; }

        public string? Exception { get; set; }
    }
}
