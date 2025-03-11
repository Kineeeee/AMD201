using MongoDB.Driver;
using Microsoft.Extensions.Options;
using URLShortener.Models;

namespace URLShortener.Data
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }

    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<UrlModel> Urls => _database.GetCollection<UrlModel>("urls");
    }
}
