using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;
public interface ITwitchMessageData
{
    Task<bool> CreateTwitchMessageData(ChannelChatMessage streamEvent);
    Task<List<ChannelChatMessage>> GetAllTwitchMessageData();
    Task<List<ChannelChatMessage>> GetTwitchMessageDataByBroadcaster(string userId);
    Task<List<ChannelChatMessage>> GetTwitchMessageDataByChatterId(string userId);
    Task<bool> RemoveTwitchMessage(string userId);
}