using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Shared.Events;

public class EventStateManager
{
    StreamEventLogModel _eventLogModel;
    IStreamEventLogData _eventLogData;

    ITwitchMessageData _twitchMessageData;

    ITwitchFollowData _twitchFollowData;
    ITwitchSubscribeData _twitchSubscribeData;
    ITwitchSubscriptionGiftData _twitchSubscriptionGiftData;
    ITwitchRaidData _twitchRaidData;
    ITwitchCheerData _twitchCheerData;

    public EventStateManager(IStreamEventLogData eventLog,
        ITwitchMessageData messageData,
        ITwitchFollowData followData,
        ITwitchSubscribeData subscribeData,
        ITwitchSubscriptionGiftData subscriptionGiftData,
        ITwitchRaidData raidData,
        ITwitchCheerData cheerData)
    {
        _eventLogData = eventLog;
        _twitchMessageData = messageData;
        _twitchFollowData = followData;
        _twitchSubscribeData = subscribeData;
        _twitchSubscriptionGiftData = subscriptionGiftData;
        _twitchRaidData = raidData;
        _twitchCheerData = cheerData;

        // Set this up in a function
        _eventLogModel = new StreamEventLogModel();
    }

    public event Action? OnChange;

    public void AddTwitchMesage(ChannelChatMessage message)
    {
        _eventLogModel.TwitchEventData.ChannelChatMessage?.Add(message);
    }

    public void AddTwitchFollow(ChannelFollow follow)
    {
        _eventLogModel.TwitchEventData.ChannelFollow?.Add(follow);
    }

    public void AddTwitchSubscribe(ChannelSubscribe subscribe)
    {
        _eventLogModel.TwitchEventData.ChannelSubscription?.Add(subscribe);
    }

    public void AddTwitchSubscriptionGift(ChannelSubscriptionGift gift)
    {
        _eventLogModel.TwitchEventData.ChannelSubscriptionGift?.Add(gift);
    }

    public void AddTwitchRaid(ChannelRaid raid)
    {
        _eventLogModel.TwitchEventData.ChannelRaid?.Add(raid);
    }

    public void AddTwitchCheer(ChannelCheer cheer)
    {
        _eventLogModel.TwitchEventData.ChannelCheer?.Add(cheer);
    }

    public async Task UpdateEventLogModel(StreamEventLogModel newEventLogModel)
    {
        await _eventLogData.UpdateEventLogData(_eventLogModel);
        NotifyOnChangeEvent();
    }

    private void NotifyOnChangeEvent() => OnChange?.Invoke();
}
