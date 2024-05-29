using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using StreamWorks.Hubs;
using Microsoft.AspNetCore.Components;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using StreamWorks.Api.Twitch.Controllers.EventSubs;
using Google.Protobuf.WellKnownTypes;

namespace StreamWorks.Connections;

public class TwitchEventSubConnection : IHostedService
{
    private readonly ILogger<TwitchEventSubConnection> Logger;
    private readonly IConfiguration Config;
    private IHubContext<TwitchHub> TwitchHubContext;
    private CancellationToken CancellationToken;
    private TwitchEventSubController TwitchInternalEventApi;

    private readonly EventSubWebsocketClient EventSubWebsocketClient;
    private TwitchAPI api = new();
    private HubConnection? twitchHub;

    private string AccessToken = "";

    // Enoci ID 103139699
    private string BroadcasterId = "";
    private string ModeratorId = "";
    private string UserId = "";

    public TwitchEventSubConnection(
        ILogger<TwitchEventSubConnection> logger,
        IConfiguration config, 
        IHubContext<TwitchHub> twitchHubContext, 
        EventSubWebsocketClient eventSubWebsocketClient,
        TwitchEventSubController twitchInternalEventApi
        )
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Config = config ?? throw new ArgumentNullException(nameof(config));
        TwitchHubContext = twitchHubContext ?? throw new ArgumentNullException(nameof(twitchHubContext));
        EventSubWebsocketClient = eventSubWebsocketClient ?? throw new ArgumentNullException(nameof(eventSubWebsocketClient));
        TwitchInternalEventApi = twitchInternalEventApi ?? throw new ArgumentNullException(nameof(twitchInternalEventApi));

        // SETUP THE API
        api.Settings.AccessToken = "";
        api.Settings.ClientId = Config["Twitch:ClientId"];
        api.Settings.Secret = Config["Twitch:ClientSecret"];

        // SETUP SIGNALR HUB CONNECTION 
        twitchHub = new HubConnectionBuilder()
         // TODO - Dont hardcode in this URL
        .WithUrl("https://localhost:7146/twitchhub")
        .WithAutomaticReconnect()
        .Build();

        // SETUP SIGNALR HUB CONNECTION EVENTS
        twitchHub.On<string, string, string>("SetupConnection", async (accessToken, userId, broadcasterId) =>
        {
            await SetupConnection(accessToken, userId, broadcasterId);
        });

        twitchHub.On("StartService", async () =>
        {
            await StartService();
        });

        twitchHub.On("StartService", async () =>
        {
            await StartService();
        });

        twitchHub.StartAsync();

        // SETUP WEBSOCKET CLIENT EVENTS
        EventSubWebsocketClient.WebsocketConnected += OnWebsocketConnected;
        EventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;
        EventSubWebsocketClient.WebsocketReconnected += OnWebsocketReconnected;
        EventSubWebsocketClient.ErrorOccurred += OnErrorOccurred;

        EventSubWebsocketClient.ChannelFollow += OnChannelFollow;
        EventSubWebsocketClient.ChannelChatMessage += OnChannelChatMessage;
    }

    private async Task OnErrorOccurred(object sender, ErrorOccuredArgs e)
    {
        Logger.LogError($"Websocket {EventSubWebsocketClient.SessionId} - Error occurred!");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        Logger.LogInformation("TwitchEventSubConnection has started.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        await EventSubWebsocketClient.DisconnectAsync();
    }

    public async Task SetupConnection(string accessToken, string twitchUserId, string broadcasterId ="")
    {
        AccessToken = accessToken;
        api.Settings.AccessToken = AccessToken;
        UserId = twitchUserId;
        ModeratorId = twitchUserId;
        BroadcasterId = broadcasterId;

        Logger.LogInformation($"Setting Access Token: {AccessToken}");
        Logger.LogInformation("TwitchEventSubConnection setup complete.");
    }

    public async Task StartService()
    {
        Logger.LogInformation("Starting Service...");
        await EventSubWebsocketClient.ConnectAsync();
    }

    private async Task OnWebsocketConnected(object sender, WebsocketConnectedArgs e)
    {
        Logger.LogInformation($"Websocket {EventSubWebsocketClient.SessionId} connected!");
        Logger.LogInformation($"Setting Subscriptions: Access Token: {api.Settings.AccessToken}");

        if (!e.IsRequestedReconnect)
        {
            // Subscribe to Topics
            Logger.LogInformation($"Trying Stream Online!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "stream.online",
                    "1",
                    new Dictionary<string, string> {
                                    { "broadcaster_user_id", BroadcasterId }
                    },
                    EventSubTransportMethod.Websocket,
                    EventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Online Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Stream Online subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }

            Logger.LogInformation($"Trying Channel Follow!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.follow",
                    "2",
                    new Dictionary<string, string> {
                        { "broadcaster_user_id", BroadcasterId },
                        { "moderator_user_id", ModeratorId }
                    },
                    EventSubTransportMethod.Websocket,
                    EventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Chat Message Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Stream Online subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }

            Logger.LogInformation($"Trying Chat Message!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.chat.message",
                    "1",
                    new Dictionary<string, string> {
                                    { "broadcaster_user_id", BroadcasterId },
                                    { "user_id", UserId }
                    },
                    EventSubTransportMethod.Websocket,
                    EventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Chat Message Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Stream Online subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }
        }
    }

    private async Task OnGetSubscriptions()
    {
        var subData = await api.Helix.EventSub.GetEventSubSubscriptionsAsync();
        await twitchHub.SendAsync("RecievedSubscriptions", subData);

        Logger.LogInformation($"Total Number of Subscriptions: {subData.Total} Total Points Used: {subData.TotalCost}");
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

    // ALL NON-CONNECTION RELATED EVENTS

    private async Task OnChannelFollow(object sender, ChannelFollowArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        Logger.LogInformation($"{eventData.UserName} followed {eventData.BroadcasterUserName} at {eventData.FollowedAt}");
    }

    private async Task OnChannelChatMessage(object sender, ChannelChatMessageArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        await twitchHub.SendAsync("RecievedChatMessage", eventData);

        Logger.LogInformation($"{eventData.ChatterUserName} typed {eventData.Message.Text}");
    }
}
