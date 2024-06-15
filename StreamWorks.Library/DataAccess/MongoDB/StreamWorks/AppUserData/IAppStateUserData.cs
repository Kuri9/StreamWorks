
namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.AppUserData;

public interface IAppStateUserData
{
    Task CreateStateData(UserAppStateDataModel userState);
    Task<List<UserAppStateDataModel>> GetAllStateData();
    Task<List<UserAppStateDataModel>> GetStateDataByUserId(Guid userId);
    Task UpdateStateData(UserAppStateDataModel userState);
}