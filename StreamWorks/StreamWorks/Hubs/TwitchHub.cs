using Microsoft.AspNetCore.SignalR;
using StreamWorks.Hubs.Interfaces;
using TwitchLib.Api.Helix.Models.EventSub;
using TwitchLib.Api.Helix.Models.Users.GetUserFollows;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Hubs;

public class TwitchHub: Hub
{
    public Task JoinGroup(string groupName)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public Task SetupConnectionRequest(Guid loggedInUserId, string accessToken, string userId)
    {
        return Clients.All.SendAsync("SetupConnection", loggedInUserId, accessToken, userId);
    }

    public Task StartServiceRequest()
    {
        return Clients.All.SendAsync("StartService");
    }

    public Task SubscriptionsRequest()
    {
        return Clients.All.SendAsync("OnGetSubscriptions");
    }

    // Non Setup-Related Requests
    public Task RecievedChatMessage(string group, ChannelChatMessage chatMessageData)
    {
        return Clients.Group(group).SendAsync("ChatMessageReceived", chatMessageData);
    }

    public Task RecievedChannelFollow(string group, ChannelFollow followData)
    {
        return Clients.Group(group).SendAsync("GetFollows", followData);
    }

    public Task RecievedSubscription(string group, ChannelSubscribe subData)
    {
        return Clients.Group(group).SendAsync("GetSubscribedEvents", subData);
    }

    public Task RecievedSubscriptionGift(string group, ChannelSubscriptionGift giftData)
    {
        return Clients.Group(group).SendAsync("GetSubscriptionGifts", giftData);
    }

    public Task RecievedChannelCheer(string group, ChannelCheer cheerData)
    {
        return Clients.Group(group).SendAsync("GetCheers", cheerData);
    }

    public Task RecievedChannelRaid(string group, ChannelRaid raidData)
    {
        return Clients.Group(group).SendAsync("GetRaid", raidData);
    }
}
