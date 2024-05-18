using StreamWorks.Library.Models.Users.Identity;

namespace StreamWorks.Library.DataAccess.MongoDB.Identity;
public class MongoIdentityUserData : IIdentityUserData
{
    private readonly IMongoCollection<StreamWorksUserModel> _users;
    public MongoIdentityUserData(IDbConnection db)
    {
        _users = db.StreamWorksUserModelCollection;
    }

    public async Task<List<StreamWorksUserModel>> GetAllUsersAsync()
    {
        // _ tells Mongo to find anything where (=>) it is "true". In this case, everything is true so get all records. 
        // Would replace the underscore with other queries to limit returned info
        var results = await _users.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<StreamWorksUserModel> GetUser(string id)
    {
        var results = await _users.FindAsync(u => u.Id.ToString() == id);
        return results.FirstOrDefault();
    }

    // Search by the Users ID we get from AzureUserAccess
    public async Task<StreamWorksUserModel> GetUserFromAuthentication(string objectId)
    {
        var results = await _users.FindAsync(u => u.Id.ToString() == objectId);
        return results.FirstOrDefault();
    }

    public Task CreateUser(StreamWorksUserModel user)
    {
        return _users.InsertOneAsync(user);
    }

    public Task UpdateUser(StreamWorksUserModel user)
    {
        var filter = Builders<StreamWorksUserModel>.Filter.Eq("Id", user.Id);
        // Upsert = If there is no entry matching, create a new one, otherwise update it
        return _users.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
    }
}
