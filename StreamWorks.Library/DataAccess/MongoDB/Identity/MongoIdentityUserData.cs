using StreamWorks.Library.Models.Users.Identity;

namespace StreamWorks.Library.DataAccess.MongoDB.Identity;
public class MongoIdentityUserData : IIdentityUserData
{
    private readonly IMongoCollection<IdentityUserModel> _users;
    public MongoIdentityUserData(IDbConnection db)
    {
        _users = db.IdentityUserCollection;
    }

    public async Task<List<IdentityUserModel>> GetAllUsersAsync()
    {
        // _ tells Mongo to find anything where (=>) it is "true". In this case, everything is true so get all records. 
        // Would replace the underscore with other queries to limit returned info
        var results = await _users.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<IdentityUserModel> GetUser(string id)
    {
        var results = await _users.FindAsync(u => u.Id == id);
        return results.FirstOrDefault();
    }

    // Search by the Users ID we get from AzureUserAccess
    public async Task<IdentityUserModel> GetUserFromAuthentication(string objectId)
    {
        var results = await _users.FindAsync(u => u.Id == objectId);
        return results.FirstOrDefault();
    }

    public Task CreateUser(IdentityUserModel user)
    {
        return _users.InsertOneAsync(user);
    }

    public Task UpdateUser(IdentityUserModel user)
    {
        var filter = Builders<IdentityUserModel>.Filter.Eq("Id", user.Id);
        // Upsert = If there is no entry matching, create a new one, otherwise update it
        return _users.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
    }
}
