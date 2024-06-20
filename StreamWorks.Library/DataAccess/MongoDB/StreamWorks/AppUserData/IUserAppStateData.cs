
namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.AppUserData;

public interface IUserAppStateData
{
    Task CreateStateData(UserAppStateModel userState);
    Task<List<UserAppStateModel>> GetAllStateData();
    Task<List<UserAppStateModel>> GetStateDataByUserId(Guid userId);
    Task UpdateStateData(UserAppStateModel userState);
}