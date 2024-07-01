using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;
public interface ITwitchCheerData
{
    Task CreateTwitchCheerData(ChannelCheer streamEvent);
    Task<List<ChannelCheer>> GetAllTwitchCheerData();
    Task<List<ChannelCheer>> GetTwitchCheerDataByBroadcasterId(string userId);
}