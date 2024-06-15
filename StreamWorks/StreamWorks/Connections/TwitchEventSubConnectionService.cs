
using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StreamWorks.Connections.Scopes;
using StreamWorks.Hubs;
using StreamWorks.Library.Models.Connections.TwitchEvent;
using System.Collections.Concurrent;

namespace StreamWorks.Connections;

public sealed class TwitchEventSubConnectionService(
        IServiceScopeFactory ServiceScopeFatory,
        ILogger<TwitchEventSubConnectionService> Logger,
        IConfiguration Config,
        IHubContext<TwitchHub> HubContext
    ) : BackgroundService
{
    private const string ClassName = nameof(TwitchEventSubConnectionService);
    private HubConnection? twitchHub;
    private string hubName = "/twitchhub";

    private readonly ConcurrentDictionary<Guid, EventSubConnectionModel> connectionsList = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation($"{ClassName} is running.");

        // SETUP SIGNALR HUB CONNECTION 
        twitchHub = new HubConnectionBuilder()
        .WithUrl(Config["BaseUrl"] + hubName)
        .WithAutomaticReconnect()
        .Build();

        // SETUP SIGNALR HUB CONNECTION EVENTS
        twitchHub.On<Guid, string, string>("SetupConnection", async (loggedInUserId, accessToken, userId) =>
        {
            await SetupScopedInstance(stoppingToken, loggedInUserId, accessToken, userId);
        });

        twitchHub.On<Guid>("RemoveConnection", async (loggedInUserId) =>
        {
            await RemoveScopedInstance(loggedInUserId);
        });

        await twitchHub.StartAsync();

        //while (!stoppingToken.IsCancellationRequested) { }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation($"TwitchEventSubConnectionService is stopping.");
        await base.StopAsync(cancellationToken);
    }

    private async Task<bool> SetupScopedInstance(CancellationToken cancellationToken, Guid loggedInUserId, string accessToken, string userId)
    {
        if(connectionsList.ContainsKey(loggedInUserId))
        {
            Logger.LogInformation($"{ClassName} already has an instance for User ID: {loggedInUserId}. Skipping Setup Process...");
            return false;
        }

        Logger.LogInformation($"{ClassName} is creating a new UserInstance. Initializing Setup Process...");
        using (IServiceScope scope = ServiceScopeFatory.CreateScope())
        {
            IScopedEventSubConnection scopedEventSubConnection = scope.ServiceProvider.GetRequiredService<IScopedEventSubConnection>();
            //Logger.LogInformation($"{ClassName} updated UserId to {userId}, Access Token to {accessToken}, and Broadcaster Id to {broadcasterId}.");

            var connectionInstance = await scopedEventSubConnection.CreateScopedEventSubConnection(cancellationToken, loggedInUserId, accessToken, userId);
            connectionInstance.StreamWorksUserId = loggedInUserId;

            if (connectionInstance is null)
            {
                Logger.LogError($"{ClassName} failed to create a new UserInstance for User ID: {loggedInUserId}. Could not add to Connection List...");
                return false;
            }
            connectionsList.AddOrUpdate(
                loggedInUserId, 
                connectionInstance,
                (k ,v) => connectionInstance);

            Logger.LogInformation($"New instance added to Dictionary: User ID: {loggedInUserId}, Model Id: {connectionsList.GetValueOrDefault(loggedInUserId).StreamWorksUserId}");
        }

        return true;
    }

    public async Task<EventSubConnectionModel> GetUserInstance(Guid userId)
    {
        if (connectionsList.TryGetValue(userId, out var connectionInstance))
        {
            return connectionInstance;
        }
        return null;
    }

    public async Task<bool> RemoveScopedInstance(Guid userId)
    {
        var isRemoved = connectionsList.TryRemove(userId, out _);
        Logger.LogInformation($"Instance removed from Dictionary: User ID: {userId}");
        return isRemoved;
    }
}
