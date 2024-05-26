using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
using StreamWorks.Api.Twitch.Controllers.User;
using TwitchLib.Api.Helix.Models.EventSub;

namespace StreamWorks.Api.Twitch.Controllers.EventSubs;
[Route("api/[controller]")]
[ApiController]
public class TwitchEventSubController : ControllerBase
{
    private readonly ILogger<TwitchUserController> _logger;
    private readonly IConfiguration _config;
    private HubConnection? hubConnection;
    private readonly TwitchAPI _twitchApi;

    public TwitchEventSubController(ILogger<TwitchUserController> logger, IConfiguration config, TwitchAPI api)
    {
        _logger = logger;
        _config = config;
        _twitchApi = api;

        _twitchApi.Settings.ClientId = _config["Twitch:ClientId"];
        _twitchApi.Settings.Secret = _config["Twitch:ClientSecret"];
    }

    [HttpGet(Name = "EventSubSubscriptions")]
    public async Task<GetEventSubSubscriptionsResponse> GetEventSubSubscriptions(string userId, string ClientId, string accessToken)
    {
        try
        {
            var subs = await _twitchApi.Helix.EventSub.GetEventSubSubscriptionsAsync(accessToken, ClientId);
            return subs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting Eventsub Subscriptions from Twitch. Error Code: {ex.HResult}");
            return null;
        }
    }
}
