using StreamWorks.Library.Models.StreamData;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Helpers.StreamData.StreamEventLogging;

public class StreamEventLogging
{
    private ILogger<StreamEventLogging> _logger;
    private IStreamEventLogData _eventLogData;

    public StreamEventLogging(ILogger<StreamEventLogging> logger, IStreamEventLogData eventLogData)
    {
        _logger = logger;
        _eventLogData = eventLogData;
    }

    public async Task<StreamEventLogModel> GetOrCreateLogData(Guid userId)
    {
        var result = await GetLog(userId);

        if(result is null || String.IsNullOrEmpty(result.Id))
        {
            var newLog = await CreateLog(userId);
            return newLog;
        }
        else
        {
            return result;
        }
    }

    public async Task<StreamEventLogModel> CreateLog(Guid userId)
    {
        var newEventLog = new StreamEventLogModel();
        newEventLog.UserId = userId;

        await _eventLogData.CreateEventLogData(newEventLog);
        return newEventLog;
    }

    public async Task UpdateLog(StreamEventLogModel eventLogData)
    {
        await _eventLogData.UpdateEventLogData(eventLogData);
    }

    public async Task<StreamEventLogModel> GetLog(Guid userId)
    {
        var result = await _eventLogData.GetEventLogDataByUserId(userId);
        if(result is not null)
        {
            return result.First();
        }
        else
        {
            _logger.LogError("No Stream Event Log found for user: {userId}", userId);
            return null;
        }
    }

    public async Task LogStreamFollow(StreamEventLogModel eventLog, ChannelFollow eventData)
    {
        if(eventLog?.TwitchEventData is not null)
        {
            eventLog.TwitchEventData.ChannelFollow.Add(eventData);
            await UpdateLog(eventLog);
        }
        else
        {
            _logger.LogError("StreamEventLog is not initialized");
        }
        
    }
}
