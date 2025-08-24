using Logger.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Logger.Domain.Data
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        // Exposing the ExceptionLogs collection
        public IMongoCollection<ExceptionLogEntity> ExceptionLogs => _database.GetCollection<ExceptionLogEntity>("ExceptionLogs");

        // Expose other collections if necessary
    }
}
