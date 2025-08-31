using TaskTrackerClean.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TaskTrackerClean.Domain.Data
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDbConnection");
            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoCollection<TaskReportEntity> TaskReports => _database.GetCollection<TaskReportEntity>("TaskReports");
    }
}
