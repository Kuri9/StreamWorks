﻿using Microsoft.AspNetCore.SignalR;
using StreamWorks.Hubs.Interfaces;
using TwitchLib.Api.Helix.Models.EventSub;
using TwitchLib.Api.Helix.Models.Users.GetUserFollows;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Hubs;

public class TwitchHub: Hub
{
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
    public Task RecievedChatMessage(ChannelChatMessage chatMessageData)
    {
        return Clients.All.SendAsync("ChatMessageReceived", chatMessageData);
    }

    public Task RecievedChannelFollow(ChannelFollow followData)
    {
        return Clients.All.SendAsync("GetFollows", followData);
    }

    public Task RecievedSubscription(ChannelSubscribe subData)
    {
        return Clients.All.SendAsync("GetSubscribedEvents", subData);
    }

    public Task RecievedSubscriptionGift(ChannelSubscriptionGift giftData)
    {
        return Clients.All.SendAsync("GetSubscriptionGifts", giftData);
    }

    public Task RecievedChannelCheer(ChannelCheer cheerData)
    {
        return Clients.All.SendAsync("GetCheers", cheerData);
    }
}
