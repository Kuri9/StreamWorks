using StreamWorks.Library.Models.Users.Identity;

namespace StreamWorks.Library.DataAccess;
public interface IDbUserConnection
{
    MongoClient Client { get; }
    string DbName { get; }
    IMongoCollection<StreamWorksRoleModel> StreamWorksRoleCollection { get; }
    string StreamWorksRoleCollectionName { get; }
    IMongoCollection<StreamWorksUserModel> StreamWorksUserModelCollection { get; }
    string StreamWorksUserModelCollectionName { get; }
}