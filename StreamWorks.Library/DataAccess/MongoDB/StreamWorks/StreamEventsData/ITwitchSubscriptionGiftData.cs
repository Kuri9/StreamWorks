using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;
public interface ITwitchSubscriptionGiftData
{
    Task CreateTwitchSubGiftData(ChannelSubscriptionGift streamEvent);
    Task<List<ChannelSubscriptionGift>> GetAllTwitchSubGiftData();
    Task<List<ChannelSubscriptionGift>> GetTwitchSubGiftDataByBroadcasterId(string userId);
}