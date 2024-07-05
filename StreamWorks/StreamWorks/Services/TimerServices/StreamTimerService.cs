
using Microsoft.AspNetCore.SignalR;
using StreamWorks.Hubs;

namespace StreamWorks.Services.TimerServices;

public class StreamTimerService : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(1000));
    private IHubContext<StreamHub> _hubContext { get; }

    public StreamTimerService(IHubContext<StreamHub> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken)
            && !stoppingToken.IsCancellationRequested)
        {
            // Do something here
            await OnTick();
            
        }
    }

    public async Task OnTick()
    {
        String currentTime = DateTime.Now.ToString("HH:mm:ss");
        await _hubContext.Clients.All.SendAsync("TimerTicked", currentTime);
    }
}
