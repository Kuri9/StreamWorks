using StreamWorks.Library.Models.Users.Identity;

namespace StreamWorks.Library.DataAccess;
public class DbConnection : IDbConnection
{
    // Create a single connection that will stay open at all times. 
    // Can change to a Scoped Singleton after to make seperate connections for multiple users if needed.
    private readonly IConfiguration _config;
    private readonly IMongoDatabase _db;
    private string _connectionId = "MongoDB";

    public MongoClient Client { get; private set; }
    public string DbName { get; private set; }

    // Collections are roughly similar to a table in SQL
    // GENERAL

    // USERS
    public string IdentityUserCollectionName { get; private set; }
    public string IdentityRoleCollectionName { get; private set; }

    public IMongoCollection<IdentityUserModel> IdentityUserCollection { get; private set; }
    public IMongoCollection<IdentityRoleModel> IdentityRoleCollection { get; private set; }

    // STREAMWORKS COLLECTIONS
    //public string StreamTimerCollectionName { get; private set; } = "stream-timer";

    //public IMongoCollection<StreamTimerModel> StreamTimerCollection { get; private set; }

    public DbConnection(IConfiguration config)
    {
        _config = config;
        Client = new MongoClient(_config.GetConnectionString(_connectionId))
            ?? throw new Exception($"Missing Connection String at {_connectionId}");
        DbName = _config["DatabaseName"];
        _db = Client.GetDatabase(DbName);

        // GENERAL

        // USERS
        IdentityUserCollection = _db.GetCollection<IdentityUserModel>(IdentityUserCollectionName);
        IdentityRoleCollection = _db.GetCollection<IdentityRoleModel>(IdentityRoleCollectionName);

        // STREAMWORKS DATA
        //StreamTimerCollection = _db.GetCollection<StreamTimerModel>(StreamTimerCollectionName);
    }
}
