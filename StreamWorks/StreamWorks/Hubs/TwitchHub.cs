using Microsoft.AspNetCore.SignalR;
using StreamWorks.Hubs.Interfaces;
using TwitchLib.Api.Helix.Models.EventSub;
using TwitchLib.Api.Helix.Models.Users.GetUserFollows;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs;

namespace StreamWorks.Hubs;

public class TwitchHub: Hub
{
    public Task JoinGroup(string groupName)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public Task SetupConnectionRequest(string group, Guid loggedInUserId, string accessToken, string userId)
    {
        return Clients.Group(group).SendAsync("SetupTwitchConnection", loggedInUserId, accessToken, userId);
    }

    public Task StartServiceRequest(string group)
    {
        return Clients.Group(group).SendAsync("StartTwitchService");
    }

    // Not sure I need these as I think just setting them up in the client is ok, but just in case...
    // =================================================================================================
    public Task OnTwitchServiceStarted(string group)
    {
        return Clients.Group(group).SendAsync("OnStartedTwitchService");
    }

    public Task OnTwitchEventSubRegistrationCompleted(string group)
    {
        return Clients.Group(group).SendAsync("OnTwicthEventSubRegistrationCompleted");
    }

    public Task OnTwitchClientDisconnected(string group)
    {
        return Clients.Group(group).SendAsync("OnTwitchClientDisconnected");
    }

    public Task OnTwitchClientReconnected(string group)
    {
        return Clients.Group(group).SendAsync("OnTwitchClientReconnected");
    }

    public Task OnTwitchError(string group, ErrorOccuredArgs e)
    {
        return Clients.Group(group).SendAsync("OnTwitchError", e);
    }
    // =================================================================================================

    public Task SubscriptionsRequest(string group)
    {
        return Clients.Group(group).SendAsync("OnGetTwitchSubscriptions");
    }

    // Non Setup-Related Requests
    // TWITCH
    public Task TwitchStreamStarted(string group, ChannelFollow streamData)
    {
        return Clients.Group(group).SendAsync("TwitchStreamStarted", streamData);
    }

    public Task TwitchStreamEnded(string group, ChannelFollow streamData)
    {
        return Clients.Group(group).SendAsync("TwitchStreamEnded", streamData);
    }

    public Task RecievedChatMessage(string group, ChannelChatMessage chatMessageData)
    {
        return Clients.Group(group).SendAsync("TwitchChatMessageReceived", chatMessageData);
    }

    public Task RecievedChannelFollow(string group, ChannelFollow followData)
    {
        return Clients.Group(group).SendAsync("GetTwitchFollows", followData);
    }

    public Task RecievedSubscription(string group, ChannelSubscribe subData)
    {
        return Clients.Group(group).SendAsync("GetTwitchSubscribedEvents", subData);
    }

    public Task RecievedSubscriptionGift(string group, ChannelSubscriptionGift giftData)
    {
        return Clients.Group(group).SendAsync("GetTwitchSubscriptionGifts", giftData);
    }

    public Task RecievedChannelCheer(string group, ChannelCheer cheerData)
    {
        return Clients.Group(group).SendAsync("GetTwitchCheers", cheerData);
    }

    public Task RecievedChannelRaid(string group, ChannelRaid raidData)
    {
        return Clients.Group(group).SendAsync("GetTwitchRaid", raidData);
    }
}
