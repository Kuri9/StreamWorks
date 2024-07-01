using System.Collections.Concurrent;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Library.Models.StreamData;
public class TwitchEventDataModel : ITwitchEventData
{
    public ConcurrentBag<ChannelChatMessage>? ChannelChatMessage { get; set; } =new ConcurrentBag<ChannelChatMessage>();
    public ConcurrentBag<ChannelChatMessageDelete>? ChannelChatMessageDelete { get; set; } = new ConcurrentBag<ChannelChatMessageDelete>();
    public ConcurrentBag<ChannelFollow>? ChannelFollow { get; set; } = new ConcurrentBag<ChannelFollow>();
    public ConcurrentBag<ChannelRaid>? ChannelRaid { get; set; } = new ConcurrentBag<ChannelRaid>();
    public ConcurrentBag<ChannelCheer>? ChannelCheer { get; set; } = new ConcurrentBag<ChannelCheer>();
    public ConcurrentBag<ChannelSubscribe>? ChannelSubscription { get; set; } = new ConcurrentBag<ChannelSubscribe>();
    public ConcurrentBag<ChannelSubscriptionEnd>? ChannelSubscriptionEnd { get; set; } = new ConcurrentBag<ChannelSubscriptionEnd>();
    public ConcurrentBag<ChannelSubscriptionGift>? ChannelSubscriptionGift { get; set; } = new ConcurrentBag<ChannelSubscriptionGift>();
    public ConcurrentBag<ChannelSubscriptionMessage>? ChannelSubscriptionMessage { get; set; } = new ConcurrentBag<ChannelSubscriptionMessage>();
}
