namespace Common.Contracts.Dtos
{
    public class EncryptedMessage : RabbitMQMesage
    {
        public required string EncryptedString { get; set; }
    }
}
