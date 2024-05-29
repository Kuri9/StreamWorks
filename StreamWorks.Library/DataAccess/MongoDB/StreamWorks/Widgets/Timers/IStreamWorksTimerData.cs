
namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.Widgets.Timers;

public interface IStreamWorksTimerData
{
    Task<List<StreamTimerModel>> GetAllTimerData();
    Task<StreamTimerModel> GetTimerDataById(string timerId);
    Task<List<StreamTimerModel>> GetTimerDataByUserId(Guid userId);
    Task CreateTimerData(StreamTimerModel timer);
    Task UpdateTimerData(StreamTimerModel timer);
}