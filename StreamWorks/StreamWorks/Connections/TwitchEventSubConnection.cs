using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using StreamWorks.Hubs;
using Microsoft.AspNetCore.Components;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StreamWorks.Connections;

public class TwitchEventSubConnection : IHostedService
{
    private readonly ILogger<TwitchEventSubConnection> Logger;
    private readonly IConfiguration Config;
    private readonly HttpClient HttpClient;
    private IHubContext<TwitchHub> TwitchHubContext;
    private CancellationToken CancellationToken;

    private readonly EventSubWebsocketClient EventSubWebsocketClient;
    private TwitchAPI api = new();
    private HubConnection? twitchHub;

    public TwitchEventSubConnection(
        ILogger<TwitchEventSubConnection> logger,
        IConfiguration config, 
        IHubContext<TwitchHub> twitchHubContext, 
        EventSubWebsocketClient eventSubWebsocketClient
        )
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Config = config ?? throw new ArgumentNullException(nameof(config));
        TwitchHubContext = twitchHubContext ?? throw new ArgumentNullException(nameof(twitchHubContext));
        EventSubWebsocketClient = eventSubWebsocketClient ?? throw new ArgumentNullException(nameof(eventSubWebsocketClient));

        api.Settings.ClientId = Config["Twitch:ClientId"];
        api.Settings.Secret = Config["Twitch:ClientSecret"];

        twitchHub = new HubConnectionBuilder()
        .WithUrl("https://localhost:7146/twitchhub")
        .WithAutomaticReconnect()
        .Build();

        twitchHub.On<string>("SetupConnection", async (accessToken) =>
        {
            await SetupConnection(accessToken);
        });

        twitchHub.On("StartService", async () =>
        {
            await StartService();
        });

        twitchHub.StartAsync();

        EventSubWebsocketClient.WebsocketConnected += OnWebsocketConnected;
        EventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;
        EventSubWebsocketClient.WebsocketReconnected += OnWebsocketReconnected;
        EventSubWebsocketClient.ErrorOccurred += OnErrorOccurred;

        EventSubWebsocketClient.ChannelFollow += OnChannelFollow;
    }

    private async Task OnErrorOccurred(object sender, ErrorOccuredArgs e)
    {
        Logger.LogError($"Websocket {EventSubWebsocketClient.SessionId} - Error occurred!");
    }

    private async Task OnChannelFollow(object sender, ChannelFollowArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        Logger.LogInformation($"{eventData.UserName} followed {eventData.BroadcasterUserName} at {eventData.FollowedAt}");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        Console.WriteLine("TwitchEventSubConnection has started.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        await EventSubWebsocketClient.DisconnectAsync();
    }

    public async Task SetupConnection(string accessToken)
    {
        api.Settings.AccessToken = accessToken;
        Console.WriteLine("TwitchEventSubConnection setup complete. Starting Service...");
    }

    public async Task StartService()
    {
        Console.WriteLine("Starting Service...");
        await EventSubWebsocketClient.ConnectAsync();
    }

    private async Task OnWebsocketConnected(object sender, WebsocketConnectedArgs e)
    {
        Logger.LogInformation($"Websocket {EventSubWebsocketClient.SessionId} connected!");

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
                    EventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Stream Online subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
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
                EventSubWebsocketClient.SessionId
                );
        }
    }

    private async Task OnWebsocketDisconnected(object sender, EventArgs e)
    {
        Logger.LogError($"Websocket {EventSubWebsocketClient.SessionId} disconnected!");
        await WebsocketReconnectAsync();
    }

    public async Task WebsocketReconnectAsync()
    {
        // Don't do this in production. You should implement a better reconnect strategy
        while (!await EventSubWebsocketClient.ReconnectAsync())
        {
            Logger.LogError("Websocket reconnect failed!");
            await Task.Delay(1000);
        }
    }

    private async Task OnWebsocketReconnected(object sender, EventArgs e)
    {
        Logger.LogWarning($"Websocket {EventSubWebsocketClient.SessionId} reconnected");
    }
}
