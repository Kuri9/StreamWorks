using StreamWorks.Library.Models.Widgets.Timers;

namespace StreamWorks.Library.DataAccess;
public class DbStreamWorksConnection : IDbStreamWorksConnection
{
    // Create a single connection that will stay open at all times. 
    // Can change to a Scoped Singleton after to make seperate connections for multiple users if needed.
    private readonly IConfiguration _config;
    private readonly IMongoDatabase _db;
    private string _connectionGroup = "MongoDbMainConfig";
    private string _connectionId = "MongoDBMain";

    public MongoClient Client { get; private set; }
    public string DbName { get; private set; }

    // Collections are roughly similar to a table in SQL
    // GENERAL

    // STREAMWORKS COLLECTIONS
    public string StreamTimerCollectionName { get; private set; } = "Stream Timer";

    public IMongoCollection<StreamTimerModel> StreamTimerCollection { get; private set; }

    public DbStreamWorksConnection(IConfiguration config)
    {
        _config = config;

        var mongoDbSettings = _config.GetSection(_connectionGroup);
        Client = new MongoClient(_config.GetConnectionString(_connectionId))
            ?? throw new Exception($"Missing Connection String at {_connectionId}");
        DbName = mongoDbSettings["DatabaseName"];
        _db = Client.GetDatabase(DbName);

        // GENERAL

        // STREAMWORKS DATA
        StreamTimerCollection = _db.GetCollection<StreamTimerModel>(StreamTimerCollectionName);
    }
}
