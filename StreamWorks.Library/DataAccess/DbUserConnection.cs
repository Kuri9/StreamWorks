using StreamWorks.Library.Models.Users.Identity;

namespace StreamWorks.Library.DataAccess;
public class DbUserConnection : IDbUserConnection
{
    // Create a single connection that will stay open at all times. 
    // Can change to a Scoped Singleton after to make seperate connections for multiple users if needed.
    private readonly IConfiguration _config;
    private readonly IMongoDatabase _db;
    private string _connectionGroup = "MongoDbUserConfig";
    private string _connectionId = "MongoDBUsers";

    public MongoClient Client { get; private set; }
    public string DbName { get; private set; }

    // Collections are roughly similar to a table in SQL
    // GENERAL

    // USERS
    public string StreamWorksUserModelCollectionName { get; private set; } = "Users";
    public string StreamWorksRoleCollectionName { get; private set; } = "Roles";

    public IMongoCollection<StreamWorksUserModel> StreamWorksUserModelCollection { get; private set; }
    public IMongoCollection<StreamWorksRoleModel> StreamWorksRoleCollection { get; private set; }

    // STREAMWORKS COLLECTIONS
    //public string StreamTimerCollectionName { get; private set; } = "stream-timer";

    //public IMongoCollection<StreamTimerModel> StreamTimerCollection { get; private set; }

    public DbUserConnection(IConfiguration config)
    {
        _config = config;

        var mongoDbSettings = _config.GetSection(_connectionGroup);
        Client = new MongoClient(_config.GetConnectionString(_connectionId))
            ?? throw new Exception($"Missing Connection String at {_connectionId}");
        DbName = mongoDbSettings["DatabaseName"];
        _db = Client.GetDatabase(DbName);

        // GENERAL

        // USERS
        StreamWorksUserModelCollection = _db.GetCollection<StreamWorksUserModel>(StreamWorksUserModelCollectionName);
        StreamWorksRoleCollection = _db.GetCollection<StreamWorksRoleModel>(StreamWorksRoleCollectionName);

        // STREAMWORKS DATA
        //StreamTimerCollection = _db.GetCollection<StreamTimerModel>(StreamTimerCollectionName);
    }
}
