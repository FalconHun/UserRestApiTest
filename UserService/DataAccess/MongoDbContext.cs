using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserService.Models;
using UserService.Options;

namespace UserService.DataAccess
{
    public class MongoDbContext
    {
        private readonly MongoDbContextOptions options;
        private readonly IMongoDatabase _database = null;

        public MongoDbContext(IOptions<MongoDbContextOptions> optionAccessor)
        {
            options = optionAccessor.Value;
            var client = new MongoClient(options.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(options.Database);
        }

        public IMongoCollection<User> Users
        {
            get
            {
                return _database.GetCollection<User>("Users");
            }
        }
    }
}
