using Microsoft.AspNetCore.SignalR;
using StreamWorks.Hubs.Interfaces;

namespace StreamWorks.Hubs;

public class TwitchHub: Hub
{
    public Task SetupConnectionRequest(string accessToken)
    {
        return Clients.All.SendAsync("SetupConnection", accessToken);
    }
}
