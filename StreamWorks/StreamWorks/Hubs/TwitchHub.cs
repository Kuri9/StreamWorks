using Microsoft.AspNetCore.SignalR;
using StreamWorks.Hubs.Interfaces;
using TwitchLib.Api.Helix.Models.EventSub;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Hubs;

public class TwitchHub: Hub
{
    public Task SetupConnectionRequest(string accessToken)
    {
        return Clients.All.SendAsync("SetupConnection", accessToken);
    }

    public Task StartServiceRequest()
    {
        return Clients.All.SendAsync("StartService");
    }

    public Task RecievedChatMessage(ChannelChatMessage chatMessageData)
    {
        return Clients.All.SendAsync("ChatMessageReceived", chatMessageData);
    }

    public Task SubscriptionsRequest()
    {
        return Clients.All.SendAsync("OnGetSubscriptions");
    }
    public Task RecievedSubscriptions(GetEventSubSubscriptionsResponse subData)
    {
        return Clients.All.SendAsync("GetSubscribedEvents", subData);
    }
}
