
namespace StreamWorks.Services;
public class BackgroundServiceManager : IHostedService
{
    ILogger<BackgroundServiceManager> logger;

    public BackgroundServiceManager(ILogger<BackgroundServiceManager> logger)
    {
        this.logger = logger;
        
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("BackgroundServiceManager has started.");
        Console.WriteLine("BackgroundServiceManager has started.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("BackgroundServiceManager has ended.");
        Console.WriteLine("BackgroundServiceManager has ended.");
    }
}
