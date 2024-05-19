using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using StreamWorks.Library.Models.Users.Identity;

namespace StreamWorks.Library.DataAccess.MongoDB.Identity;
public class MongoStreamWorksUserData : IStreamWorksUserData
{
    private readonly IMongoCollection<StreamWorksUserModel> _users;
    public MongoStreamWorksUserData(IDbConnection db)
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
        if (string.IsNullOrEmpty(id))
        {
            // TODO: Implement better logging and handle better than just returning Null
            Console.WriteLine("GetUser: Id was empty.");
            return null;

        } else {
            Guid objectIdGuid = Guid.Parse(id); // Convert the objectId string to a Guid
            var filter = Builders<StreamWorksUserModel>.Filter.Eq(u => u.Id, objectIdGuid); // Compare the Guids
            var results = await _users.FindAsync(filter);
            return results.FirstOrDefault();
        }
    }

    // Search by the Users ID we get from AzureUserAccess
    public async Task<StreamWorksUserModel> GetUserFromAuthentication(string objectId)
    {
        if (string.IsNullOrEmpty(objectId))
        {
            // TODO: Implement better logging and handle better than just returning Null
            Console.WriteLine("GetUserFromAuthentication: objectId was empty.");
            return null;
        }
        else
        {
            Guid objectIdGuid = Guid.Parse(objectId); // Convert the objectId string to a Guid
            var filter = Builders<StreamWorksUserModel>.Filter.Eq(u => u.Id, objectIdGuid); // Compare the Guids
            var results = await _users.FindAsync(filter);
            return results.FirstOrDefault();
        }
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
