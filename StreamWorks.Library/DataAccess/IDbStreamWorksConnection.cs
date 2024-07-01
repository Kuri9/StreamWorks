using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.DataAccess;

public interface IDbStreamWorksConnection
{
    MongoClient Client { get; }
    string DbName { get; }

    IMongoCollection<UserAppStateModel> UserAppStateDataCollection { get; }
    IMongoCollection<StreamTimerModel> StreamTimerCollection { get; }
    IMongoCollection<StreamEventLogModel> StreamEventLogDataCollection { get; }

    string UserAppStateDataCollectionName { get; }
    string StreamEventLogDataCollectionName { get; }
    string StreamTimerCollectionName { get; }

    // TWITCH EVENTS
    IMongoCollection<ChannelFollow> TwitchFollowDataCollection { get; }
    IMongoCollection<ChannelSubscribe> TwitchSubscribeDataCollection { get; }
    IMongoCollection<ChannelSubscriptionEnd> TwitchEndSubscribeDataCollection { get; }
    IMongoCollection<ChannelSubscriptionGift> TwitchSubscriptionGiftDataCollection { get; }
    IMongoCollection<ChannelCheer> TwitchCheerDataCollection { get; }
    IMongoCollection<ChannelRaid> TwitchRaidCollection { get; }
    IMongoCollection<ChannelChatMessage> TwitchMessageDataCollection { get; }

    string TwitchFollowDataCollectionName { get; }
    string TwitchSubscribeDataCollectionName { get; }
    string TwitchEndSubscribeDataCollectionName { get; }
    string TwitchSubscriptionGiftDataCollectionName { get; }
    string TwitchCheerDataCollectionName { get; }
    string TwitchRaidCollectionName { get; }
    string TwitchMessageDataCollectionName { get; }
}