using StreamWorks.Library.Models.Users.Identity;

namespace StreamWorks.Library.DataAccess.MongoDB.Identity;
public interface IIdentityUserData
{
    Task CreateUser(StreamWorksUserModel user);
    Task<List<StreamWorksUserModel>> GetAllUsersAsync();
    Task<StreamWorksUserModel> GetUser(string id);
    Task<StreamWorksUserModel> GetUserFromAuthentication(string objectId);
    Task UpdateUser(StreamWorksUserModel user);
}