using StreamWorks.Library.Models.Users.UserData;

namespace StreamWorks.Core.App;
public interface IAppStateCore
{
    Task<UserAppStateModel> GetUserStateData();
    Task SetUserData();
    Task UpdateUserStateData();
}