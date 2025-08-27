using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Logger.Domain.Entities
{
    public class ExceptionLogEntity
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("service")]
        public string? Service { get; set; }

        [BsonElement("level")]
        public string? Level { get; set; }

        [BsonElement("message")]
        public string? Message { get; set; }

        [BsonElement("exception")]
        public string? Exception { get; set; }

        [BsonElement("traceId")]
        public string? TraceId { get; set; }

    }
}
