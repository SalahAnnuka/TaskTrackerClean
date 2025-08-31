using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskTrackerClean.Domain.Entities
{
    public class TaskReportEntity
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId id { get; set; }

        [BsonElement("total")]
        public int TotalTasks { get; set; }

        [BsonElement("finished")]
        public int FinishedTasks { get; set; }

        [BsonElement("unfinished")]
        public int UnfinishedTasks { get; set; }

        [BsonElement("overdue")]
        public int OverdueTasks { get; set; }

        [BsonElement("percentage")]
        public double FinishedPercentage { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
