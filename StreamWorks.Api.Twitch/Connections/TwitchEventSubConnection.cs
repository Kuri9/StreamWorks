using Google.Protobuf.WellKnownTypes;
using StreamWorks.ApiLibrary.Twitch.Models.Config;
using System.Threading;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

namespace StreamWorks.Api.Twitch.Connections;

public class TwitchEventSubConnection : IHostedService
{
    private readonly ILogger<TwitchEventSubConnection> _logger;
    private readonly IConfiguration _config;
    private CancellationToken _cancellationToken;

    private readonly EventSubWebsocketClient _eventSubWebsocketClient;
    private TwitchAPI api = new();

    private TwitchConnectionModel userConnectionData;

    public TwitchEventSubConnection(ILogger<TwitchEventSubConnection> logger, IConfiguration config, EventSubWebsocketClient eventSubWebsocketClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config ?? throw new ArgumentNullException(nameof(config));

        api.Settings.ClientId =  _config["Twitch:ClientId"];
        api.Settings.Secret = _config["Twitch:ClientSecret"]; 

        _eventSubWebsocketClient = eventSubWebsocketClient ?? throw new ArgumentNullException(nameof(eventSubWebsocketClient));
        _eventSubWebsocketClient.WebsocketConnected += OnWebsocketConnected;
        _eventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;
        _eventSubWebsocketClient.WebsocketReconnected += OnWebsocketReconnected;
        _eventSubWebsocketClient.ErrorOccurred += OnErrorOccurred;

        _eventSubWebsocketClient.ChannelFollow += OnChannelFollow;
    }

    private async Task OnErrorOccurred(object sender, ErrorOccuredArgs e)
    {
        _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} - Error occurred!");
    }

    private async Task OnChannelFollow(object sender, ChannelFollowArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        _logger.LogInformation($"{eventData.UserName} followed {eventData.BroadcasterUserName} at {eventData.FollowedAt}");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        await _eventSubWebsocketClient.ConnectAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        await _eventSubWebsocketClient.DisconnectAsync();
    }

    public async Task SetUserCredentials(TwitchConnectionModel connectionData, string accessToken)
    {
        api.Settings.AccessToken = accessToken;
        userConnectionData = connectionData;
    }

    private async Task OnWebsocketConnected(object sender, WebsocketConnectedArgs e)
    {
        _logger.LogInformation($"Websocket {_eventSubWebsocketClient.SessionId} connected!");

        if (!e.IsRequestedReconnect)
        {
            // Subscribe to Topics
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "stream.online",
                    "1",
                    new Dictionary<string, string> {
                                    { "broadcaster_user_id", "103139699" }
                    },
                    EventSubTransportMethod.Websocket,
                    _eventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex.Message.Contains("already exists"))
                {
                    _logger.LogWarning("Stream Online subscription already exists");
                }
                else if(ex.HResult == 401) 
                {
                    _logger.LogWarning("Refreshing Token");
                }
            }

            await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                "channel.follow",
                "2",
                new Dictionary<string, string> {
                    { "broadcaster_user_id", "103139699" },
                    { "moderator_user_id", "158740154" }
                },
                EventSubTransportMethod.Websocket,
                _eventSubWebsocketClient.SessionId
                );
        }
    }

    private async Task OnWebsocketDisconnected(object sender, EventArgs e)
    {
        _logger.LogError($"Websocket {_eventSubWebsocketClient.SessionId} disconnected!");
        await WebsocketReconnectAsync();
    }

    public async Task WebsocketReconnectAsync()
    {
        // Don't do this in production. You should implement a better reconnect strategy
        while (!await _eventSubWebsocketClient.ReconnectAsync())
        {
            _logger.LogError("Websocket reconnect failed!");
            await Task.Delay(1000);
        }
    }

    private async Task OnWebsocketReconnected(object sender, EventArgs e)
    {
        _logger.LogWarning($"Websocket {_eventSubWebsocketClient.SessionId} reconnected");
    }
}
