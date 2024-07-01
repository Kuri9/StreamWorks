using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;
public interface ITwitchFollowData
{
    Task CreateTwitchFollowData(ChannelFollow streamEvent);
    Task<List<ChannelFollow>> GetAllTwitchFollowData();
    Task<List<ChannelFollow>> GetTwitchFollowDataByBroadcasterId(string userId);
}