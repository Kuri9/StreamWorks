using StreamWorks.Library.Models.Users.Identity;

namespace StreamWorks.Library.DataAccess.MongoDB.Identity;
public interface IIdentityUserData
{
    Task CreateUser(IdentityUserModel user);
    Task<List<IdentityUserModel>> GetAllUsersAsync();
    Task<IdentityUserModel> GetUser(string id);
    Task<IdentityUserModel> GetUserFromAuthentication(string objectId);
    Task UpdateUser(IdentityUserModel user);
}