using StreamWorks.Library.Models.Users.Identity;

namespace StreamWorks.Library.DataAccess;
public interface IDbConnection
{
    MongoClient Client { get; }
    string DbName { get; }
    IMongoCollection<IdentityRoleModel> IdentityRoleCollection { get; }
    string IdentityRoleCollectionName { get; }
    IMongoCollection<IdentityUserModel> IdentityUserCollection { get; }
    string IdentityUserCollectionName { get; }
}