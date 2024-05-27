using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI;
using StreamWorks.Api.Twitch.Controllers.User;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.EventSub;
using TwitchLib.EventSub.Websockets;

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

    [HttpPost(Name = "SubscribeToChat")]
    public async Task GetChatEventSubscription(string userId, string broadcasterId, string sessionId, string accessToken)
    {
        try
        {
            _twitchApi.Settings.AccessToken = accessToken;

            await _twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                "channel.chat.message",
                "1",
                new Dictionary<string, string> {
                                    { "broadcaster_user_id", broadcasterId },
                                    { "user_id", userId }
                },
                EventSubTransportMethod.Websocket,
                sessionId
                );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chat Message Subscription Failed:  {ex.ToString()}");

            //if (ex.Message.Contains("already exists"))
            //{
            //    return BadRequest("Stream Online subscription already exists");
            //    _logger.LogWarning("Stream Online subscription already exists");
            //}
            //else if (ex.HResult == 401)
            //{
            //    return StatusCode(401);
            //    _logger.LogWarning("Must refresh Token");
            //}
        }
    }
}
