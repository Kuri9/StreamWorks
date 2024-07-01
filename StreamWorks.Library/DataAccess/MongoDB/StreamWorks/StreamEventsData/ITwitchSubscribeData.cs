using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;
public interface ITwitchSubscribeData
{
    Task CreateTwitchSubData(ChannelSubscribe streamEvent);
    Task<List<ChannelSubscribe>> GetAllTwitchSubData();
    Task<List<ChannelSubscribe>> GetTwitchSubDataByBroadcasterId(string userId);
}