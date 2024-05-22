using StreamWorks.Library.Models.Users.Identity;
using System.Security.Claims;

namespace StreamWorks.Library.DataAccess.MongoDB.Identity;
public interface IStreamWorksUserData
{
    Task CreateUser(StreamWorksUserModel user);
    Task<List<StreamWorksUserModel>> GetAllUsersAsync();
    Task<StreamWorksUserModel> GetUser(string id);
    Task<StreamWorksUserModel> GetUserFromAuthentication(Guid objectId);
    Task<StreamWorksUserModel> GetUserFromPrincipal(ClaimsPrincipal principal);
    Task UpdateUser(StreamWorksUserModel user);
}