
using Azure.Core;
using StreamWorks.Components;
using StreamWorks.Hubs;
using StreamWorks.Library.Models.Connections.TwitchEvent;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Moderation.GetModerators;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Stream;

namespace StreamWorks.Connections.Scopes;

public sealed class ScopedEventSubConnectionTasks(
    ILogger<ScopedEventSubConnectionTasks> Logger,
    IConfiguration Config
    ) : IScopedEventSubConnection
{
    private EventSubWebsocketClient? eventSubWebsocketClient;
    private EventSubConnectionModel? eventSubConnectionModel;
    private TwitchAPI? api;

    private Guid ThisUser;
    private string messageGroup;

    private HubConnection? twitchHub;
    private string hubName = "/twitchhub";
    private string? baseUrl;

    // Set to true to use the Test Server
    private bool isTesting = false;
    private Uri TestServer = new Uri("ws://127.0.0.1:8080/ws");

    private string AccessToken = "";
    private string BroadcasterId = "";
    private string ModeratorId = "";
    private string UserId = "";

    public async Task<EventSubConnectionModel> CreateScopedEventSubConnection(CancellationToken cancellationToken, Guid loggedInUserId, string accessToken, string twitchUserId)
    {
        Logger.LogInformation("ScopedEventSubConnectionTasks has started.");
        ThisUser = loggedInUserId;
        messageGroup = ThisUser.ToString();

        eventSubConnectionModel = new EventSubConnectionModel();

        api = new TwitchAPI();
        if (api is null)
        {
            Logger.LogError($"TwitchAPI Client {nameof(api)} is null.");
            return null;
        }
        else
        {
            await SetupConnection(accessToken, twitchUserId);
            eventSubConnectionModel.TwitchApiClient = api;
            Logger.LogInformation($"TwitchAPI Created For: {api.Settings.ClientId}");
        }

        eventSubWebsocketClient = new EventSubWebsocketClient();
        if (eventSubWebsocketClient is null)
        {
            Logger.LogError($"EventSubWebsocketClient{nameof(eventSubWebsocketClient)} is null.");
            return null;
        }
        else
        {
            Logger.LogInformation($"TwitchEventClient Created For: {api.Settings.ClientId}");
            eventSubConnectionModel.EventSubWebsocketClient = eventSubWebsocketClient;
        }

        // Setup Client Hub Connection
        await SetupHub(cancellationToken);


        // SETUP WEBSOCKET CLIENT EVENTS
        eventSubWebsocketClient.WebsocketConnected += OnWebsocketConnected;
        eventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;
        eventSubWebsocketClient.WebsocketReconnected += OnWebsocketReconnected;
        eventSubWebsocketClient.ErrorOccurred += OnErrorOccurred;

        eventSubWebsocketClient.StreamOnline += OnStreamOnline;
        eventSubWebsocketClient.StreamOffline += OnStreamOffline;

        eventSubWebsocketClient.ChannelFollow += OnChannelFollow;

        eventSubWebsocketClient.ChannelChatMessage += OnChannelChatMessage;
        //EventSubWebsocketClient.ChannelChatMessageDeleted += OnChannelChatMessageDeleted;
        //EventSubWebsocketClient.ChannelChatNotification += OnChannelChatNotification;
        //EventSubWebsocketClient.ChannelChatSettingsChanged += OnChannelChatSettingsChanged;

        eventSubWebsocketClient.ChannelSubscribe += OnChannelSubscribe;
        eventSubWebsocketClient.ChannelSubscriptionEnd += OnChannelSubscriptionEnd;
        eventSubWebsocketClient.ChannelSubscriptionGift += OnChannelSubscriptionGift;
        eventSubWebsocketClient.ChannelSubscriptionMessage += OnChannelSubscriptionMessage;

        eventSubWebsocketClient.ChannelCheer += OnChannelCheer;

        eventSubWebsocketClient.ChannelRaid += OnChannelRaid;

        await StartService();

        return eventSubConnectionModel;
    }

    private async Task SetupHub(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Setting up Hub Connection.");

        baseUrl = Config.GetValue<string>("BaseUrl") ?? "< No Url Set >";

        // SETUP SIGNALR HUB CONNECTION 
        twitchHub = new HubConnectionBuilder()
        .WithUrl(baseUrl + hubName)
        .WithAutomaticReconnect()
        .Build();

        // SETUP SIGNALR HUB CONNECTION EVENTS
        twitchHub.On<string, string, string>("SetupConnection", async (accessToken, userId, broadcasterId) =>
        {
            //await (accessToken, userId, broadcasterId);
        });

        //twitchHub.On("StartService", async () =>
        //{
        //    await StartService();
        //});

        await twitchHub.StartAsync();
        
        if (twitchHub.ConnectionId is not null)
        {
            eventSubConnectionModel.HubConnection = twitchHub;
            Logger.LogInformation($"Hub Connection Id: {twitchHub.ConnectionId}");
        }
    }

    public async Task SetupConnection(string accessToken, string twitchUserId)
    {
        AccessToken = accessToken;
        UserId = twitchUserId;
        ModeratorId = twitchUserId;
        BroadcasterId = twitchUserId;

        api.Settings.AccessToken = AccessToken;
        api.Settings.ClientId = Config["Twitch:ClientId"];
        api.Settings.Secret = Config["Twitch:ClientSecret"]; ;

        Logger.LogInformation($"Setting Access Token: {AccessToken}");
        Logger.LogInformation("TwitchEventSubConnection setup complete.");
    }

    public async Task StartService()
    {
        Logger.LogInformation("Starting Service...");
        if (isTesting == true)
        {
            Logger.LogInformation("Switching to Test Service...");
            await eventSubWebsocketClient.ConnectAsync(TestServer);
        }
        else
        {
            await eventSubWebsocketClient.ConnectAsync();
        }
    }

    private async Task OnWebsocketConnected(object sender, WebsocketConnectedArgs e)
    {
        Logger.LogInformation($"Websocket {eventSubWebsocketClient.SessionId} connected!");
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
                    eventSubWebsocketClient.SessionId
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

            Logger.LogInformation($"Trying Stream Offline!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "stream.offline",
                    "1",
                    new Dictionary<string, string> {
                                    { "broadcaster_user_id", BroadcasterId }
                    },
                    EventSubTransportMethod.Websocket,
                    eventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Offilne Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Stream Offline subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }

            // FOLLOW EVENTS
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
                    eventSubWebsocketClient.SessionId
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

            // CHAT EVENTS
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
                    eventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Chat Message Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Chat Message Subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }

            Logger.LogInformation($"Trying Chat Message Deleted!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.chat.message_delete",
                    "1",
                    new Dictionary<string, string> {
                                    { "broadcaster_user_id", BroadcasterId },
                                    { "user_id", UserId }
                    },
                    EventSubTransportMethod.Websocket,
                    eventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Chat Message Deleted Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Chat Message Deleted Subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }

            Logger.LogInformation($"Trying Channel Chat Notification!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.chat.notification",
                    "1",
                    new Dictionary<string, string> {
                                    { "broadcaster_user_id", BroadcasterId },
                                    { "user_id", UserId }
                    },
                    EventSubTransportMethod.Websocket,
                    eventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Channel Chat Notification Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Channel Chat Notification Subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }

            Logger.LogInformation($"Trying Channel Chat Settings Changed Notification!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.chat_settings.update",
                    "1",
                    new Dictionary<string, string> {
                                    { "broadcaster_user_id", BroadcasterId },
                                    { "user_id", UserId }
                    },
                    EventSubTransportMethod.Websocket,
                    eventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Channel Chat Settings Changed Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Channel Chat Settings Changed Subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }

            // SUBSCRIPTION EVENTS 
            Logger.LogInformation($"Trying Channel Subscription Notification!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.subscribe",
                    "1",
                    new Dictionary<string, string> {
                        { "broadcaster_user_id", BroadcasterId }
                    },
                    EventSubTransportMethod.Websocket,
                    eventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Channel Subscriptions Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Channel Subscriptions Subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }

            Logger.LogInformation($"Trying Channel Subscriptions Ended Notification!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.subscription.end",
                    "1",
                    new Dictionary<string, string> {
                        { "broadcaster_user_id", BroadcasterId }
                    },
                    EventSubTransportMethod.Websocket,
                    eventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Channel Subscriptions Ended Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Channel Subscriptions Ended Subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }

            Logger.LogInformation($"Trying Channel Subscription Gift Notification!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.subscription.gift",
                    "1",
                    new Dictionary<string, string> {
                        { "broadcaster_user_id", BroadcasterId }
                    },
                    EventSubTransportMethod.Websocket,
                    eventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Channel Subscription Gift Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Channel Subscription Gift Subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }

            Logger.LogInformation($"Trying Channel ReSubscription Message Notification!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.subscription.message",
                    "1",
                    new Dictionary<string, string> {
                        { "broadcaster_user_id", BroadcasterId }
                    },
                    EventSubTransportMethod.Websocket,
                    eventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Channel ReSubscription Message Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Channel ReSubscription Message Subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }

            // CHEER EVENTS
            Logger.LogInformation($"Trying Channel Cheer Notification!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.cheer",
                    "1",
                    new Dictionary<string, string> {
                        { "broadcaster_user_id", BroadcasterId }
                    },
                    EventSubTransportMethod.Websocket,
                    eventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Channel Cheer Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Channel Cheer Subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }

            // RAID EVENTS
            Logger.LogInformation($"Trying Channel Raid Message Notification!");
            try
            {
                await api.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.raid",
                    "1",
                    new Dictionary<string, string> {
                        { "to_broadcaster_user_id", BroadcasterId }
                    },
                    EventSubTransportMethod.Websocket,
                    eventSubWebsocketClient.SessionId
                    );
            }
            catch (Exception ex)
            {
                Logger.LogError($"Channel Raid Subscription Failed:  {ex.ToString()}");

                if (ex.Message.Contains("already exists"))
                {
                    Logger.LogWarning("Channel Raid Subscription already exists");
                }
                else if (ex.HResult == 401)
                {
                    Logger.LogWarning("Must refresh Token");
                }
            }
        }
    }

    private async Task OnWebsocketDisconnected(object sender, EventArgs e)
    {
        Logger.LogError($"Websocket {eventSubWebsocketClient.SessionId} disconnected!");
        await WebsocketReconnectAsync();
    }

    private async Task OnWebsocketReconnected(object sender, EventArgs e)
    {
        Logger.LogWarning($"Websocket {eventSubWebsocketClient.SessionId} reconnected");
    }

    private async Task OnErrorOccurred(object sender, ErrorOccuredArgs e)
    {
        Logger.LogError($"Websocket {eventSubWebsocketClient.SessionId} - Error occurred!");
    }

    public async Task WebsocketReconnectAsync()
    {
        // Don't do this in production. You should implement a better reconnect strategy
        while (!await eventSubWebsocketClient.ReconnectAsync())
        {
            Logger.LogError("Websocket reconnect failed!");
            await Task.Delay(1000);
        }
    }

    // ALL NON-CONNECTION RELATED EVENTS
    private async Task OnStreamOnline(object sender, StreamOnlineArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        Logger.LogInformation($"{eventData.BroadcasterUserName} started thier stream.");
    }

    private async Task OnStreamOffline(object sender, StreamOfflineArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        Logger.LogInformation($"{eventData.BroadcasterUserName} ended their stream.");
    }

    // FOLLOWS
    private async Task OnChannelFollow(object sender, ChannelFollowArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        if (twitchHub is not null)
        {
            await twitchHub.SendAsync("RecievedChannelFollow", eventData);
        }
        Logger.LogInformation($"{eventData.UserName} followed {eventData.BroadcasterUserName} at {eventData.FollowedAt}");
    }

    // MESSAGES
    private async Task OnChannelChatMessage(object sender, ChannelChatMessageArgs e)
    {
        var eventData = e.Notification.Payload.Event;


        if (twitchHub is not null)
        {
            await twitchHub.SendAsync("RecievedChatMessage", messageGroup, eventData);
        }

        Logger.LogInformation($"{eventData.ChatterUserName} typed {eventData.Message.Text}");
    }

    // SUBSCRIPTIONS
    private async Task OnChannelSubscribe(object sender, ChannelSubscribeArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        if (twitchHub is not null)
        {
            await twitchHub.SendAsync("RecievedSubscription", eventData);
        }

        Logger.LogInformation($"{eventData.UserName} subscribed to {eventData.BroadcasterUserName} with a Tier {eventData.Tier} Sub");
    }

    private async Task OnChannelSubscriptionEnd(object sender, ChannelSubscriptionEndArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        if (twitchHub is not null)
        {
            await twitchHub.SendAsync("RecievedSubscriptionEnding", eventData);
        }

        Logger.LogInformation($"{eventData.UserName} unsubscribed from {eventData.BroadcasterUserName}: It was a Tier {eventData.Tier} Sub");
    }

    private async Task OnChannelSubscriptionGift(object sender, ChannelSubscriptionGiftArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        if (twitchHub is not null)
        {
            await twitchHub.SendAsync("RecievedSubscriptionGift", eventData);
        }

        Logger.LogInformation($"{eventData.UserName} gifted {eventData.BroadcasterUserName}'s channel a Tier {eventData.Tier} Sub");
    }

    private async Task OnChannelSubscriptionMessage(object sender, ChannelSubscriptionMessageArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        if (twitchHub is not null)
        {
            await twitchHub.SendAsync("RecievedSubscriptionMessage", eventData);
        }

        Logger.LogInformation($"{eventData.UserName} sent a sub message to {eventData.BroadcasterUserName} with a Tier {eventData.Tier} Sub");
    }

    // CHEER
    private async Task OnChannelCheer(object sender, ChannelCheerArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        if (twitchHub is not null)
        {
            await twitchHub.SendAsync("RecievedChannelCheer", eventData);
        }

        Logger.LogInformation($"{eventData.UserName} cheered {eventData.Bits} Bits to {eventData.BroadcasterUserName}");
    }

    // RAID
    private async Task OnChannelRaid(object sender, ChannelRaidArgs e)
    {
        var eventData = e.Notification.Payload.Event;
        if (twitchHub is not null)
        {
            await twitchHub.SendAsync("RecievedChannelRaid", eventData);
        }

        Logger.LogInformation($"{eventData.FromBroadcasterUserName} raided {eventData.ToBroadcasterUserName} with {eventData.Viewers} Viewers!");
    }
}
