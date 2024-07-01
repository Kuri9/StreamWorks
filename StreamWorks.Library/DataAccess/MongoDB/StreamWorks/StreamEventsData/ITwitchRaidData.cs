using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;
public interface ITwitchRaidData
{
    Task CreateTwitchRaidData(ChannelRaid streamEvent);
    Task<List<ChannelRaid>> GetAllTwitchRaidData();
    Task<List<ChannelRaid>> GetTwitchRaidDataByBroadcasterId(string userId);
    Task<List<ChannelRaid>> GetTwitchRaidDataByRaidingBroadcaster(string userId);
}