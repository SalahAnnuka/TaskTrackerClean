using Logger.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Logger.Domain.Data
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;
        private readonly string _collectionName;

        public MongoDbService(IConfiguration configuration)
        {
            var mongoBuilder = new MongoUrlBuilder
            {
                Server = new MongoServerAddress(
                    configuration["DatabaseSettings:MongoDb:Host"],
                    int.Parse(configuration["DatabaseSettings:MongoDb:Port"]!)
                ),
                DatabaseName = configuration["DatabaseSettings:MongoDb:Database"]
            };

            var mongoUrl = mongoBuilder.ToMongoUrl();
            var mongoClient = new MongoClient(mongoUrl);
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _collectionName = configuration["DatabaseSettings:MongoDb:CollectionName"]!;
        }

        public IMongoCollection<ExceptionLogEntity> ExceptionLogs => _database.GetCollection<ExceptionLogEntity>(_collectionName);
    }
}
