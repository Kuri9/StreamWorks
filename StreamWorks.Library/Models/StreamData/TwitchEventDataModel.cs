using System.Collections.Concurrent;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.Models.StreamData;
public class TwitchEventDataModel
{
    public ConcurrentBag<ChannelChatMessage>? ChannelChatMessage { get; set; }
    public ConcurrentBag<ChannelChatMessageDelete>? ChannelChatMessageDelete { get; set; }
    public ConcurrentBag<ChannelFollow>? ChannelFollow { get; set; }
    public ConcurrentBag<ChannelRaid>? ChannelRaid { get; set; }
    public ConcurrentBag<ChannelCheer>? ChannelCheer { get; set; }
    public ConcurrentBag<ChannelSubscribe>? ChannelSubscription { get; set; }
    public ConcurrentBag<ChannelSubscriptionEnd>? ChannelSubscriptionEnd { get; set; }
    public ConcurrentBag<ChannelSubscriptionGift>? ChannelSubscriptionGift { get; set; }
    public ConcurrentBag<ChannelSubscriptionMessage>? ChannelSubscriptionMessage { get; set; }
}
