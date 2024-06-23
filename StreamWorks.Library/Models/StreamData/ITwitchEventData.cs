using System.Collections.Concurrent;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.Models.StreamData;
public interface ITwitchEventData
{
    ConcurrentBag<ChannelChatMessage>? ChannelChatMessage { get; set; }
    ConcurrentBag<ChannelChatMessageDelete>? ChannelChatMessageDelete { get; set; }
    ConcurrentBag<ChannelCheer>? ChannelCheer { get; set; }
    ConcurrentBag<ChannelFollow>? ChannelFollow { get; set; }
    ConcurrentBag<ChannelRaid>? ChannelRaid { get; set; }
    ConcurrentBag<ChannelSubscribe>? ChannelSubscription { get; set; }
    ConcurrentBag<ChannelSubscriptionEnd>? ChannelSubscriptionEnd { get; set; }
    ConcurrentBag<ChannelSubscriptionGift>? ChannelSubscriptionGift { get; set; }
    ConcurrentBag<ChannelSubscriptionMessage>? ChannelSubscriptionMessage { get; set; }
}