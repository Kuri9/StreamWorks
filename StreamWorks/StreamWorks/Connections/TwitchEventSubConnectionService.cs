
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
        IConfiguration Config
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

        await twitchHub.StartAsync();

        //while (!stoppingToken.IsCancellationRequested) { }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation($"TwitchEventSubConnectionService is stopping.");
        await base.StopAsync(cancellationToken);
    }

    private async Task SetupScopedInstance(CancellationToken cancellationToken, Guid loggedInUserId, string accessToken, string userId)
    {
        Logger.LogInformation($"{ClassName} is creating a new UserInstance. Initializing Setup Process...");
        using (IServiceScope scope = ServiceScopeFatory.CreateScope())
        {
            IScopedEventSubConnection scopedEventSubConnection = scope.ServiceProvider.GetRequiredService<IScopedEventSubConnection>();
            //Logger.LogInformation($"{ClassName} updated UserId to {userId}, Access Token to {accessToken}, and Broadcaster Id to {broadcasterId}.");

            var connectionInstance = await scopedEventSubConnection.CreateScopedEventSubConnection(cancellationToken, loggedInUserId, accessToken, userId);
            connectionInstance.StreamWorksUserId = loggedInUserId;

            connectionsList.AddOrUpdate(
                loggedInUserId, 
                connectionInstance,
                (k ,v) => connectionInstance);

            Logger.LogInformation($"New instance added to Dictionary: User ID: {loggedInUserId}, Model Id: {connectionsList.GetValueOrDefault(loggedInUserId).StreamWorksUserId}");
        }
    }
}
