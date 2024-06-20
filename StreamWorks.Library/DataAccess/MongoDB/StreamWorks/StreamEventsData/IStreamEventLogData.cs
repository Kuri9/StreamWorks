
namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;

public interface IStreamEventLogData
{
    Task CreateEventLogData(StreamEventLogModel streamEventLog);
    Task<List<StreamEventLogModel>> GetAllEventLogData();
    Task<List<StreamEventLogModel>> GetEventLogDataByUserId(Guid userId);
    Task UpdateEventLogData(StreamEventLogModel streamEventLog);
}