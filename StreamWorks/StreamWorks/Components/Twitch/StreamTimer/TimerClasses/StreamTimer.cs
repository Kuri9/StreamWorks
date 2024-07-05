using Microsoft.AspNetCore.SignalR;
using StreamWorks.Hubs;
using StreamWorks.Library.Models.Connections.TwitchEvent;
using StreamWorks.Library.Models.Widgets.Timers;
using StreamWorks.Library.Models.Widgets.Timers.TimerModels;
using StreamWorks.Models.Widgets.Timers;
using System.Numerics;
using System.Runtime.CompilerServices;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Components.Twitch.StreamTimer.TimerClasses;

public class StreamTimer : IStreamTimer
{
    private StreamWorksUserModel loggedInUser = default!;
    private StreamTimerModel streamTimerData = new();

    ILogger<StreamTimer> Logger;
    IConfiguration Config;

    private TimeSpan oneSecond = TimeSpan.FromSeconds(1);
    public StreamTimerModel Timer => streamTimerData;

    //public event EventHandler<TimeSpan>? TimerTickEvent;
    private IHubContext<StreamHub> _hubContext { get; }

    public StreamTimer(ILogger<StreamTimer> logger, IConfiguration config, IHubContext<StreamHub> hubContext)
    {
        Logger = logger;
        Config = config;
        _hubContext = hubContext;

        streamTimerData.TimerSettings.StartingTime = TimeSpan.FromSeconds(300);
        streamTimerData.TimerSettings.IsRunning = false;

        Setup();
    }

    private async Task Setup()
    {
        streamTimerData.CurrentTime = streamTimerData.TimerSettings.StartingTime;
    }

    public async Task OnTimerTicked()
    {
        //await _hubContext.Clients.All.SendAsync("TimerTicked");
        //Logger.LogInformation("Timer Ticked");

        if (streamTimerData.TimerSettings.IsRunning == true)
        {
            if (streamTimerData.TimerSettings.IsCountDown == true)
            {
                streamTimerData.CurrentTime -= oneSecond;
                streamTimerData.TimeElapsed += oneSecond;

                if (streamTimerData.CurrentTime <= TimeSpan.Zero)
                {
                    StopTimer();
                }

                Logger.LogInformation($"Current Time: {streamTimerData.CurrentTime}");
            }
            else
            {
                streamTimerData.CurrentTime += oneSecond;
                streamTimerData.TimeElapsed += oneSecond;

                if (streamTimerData.CurrentTime >= TimeSpan.Zero)
                {
                    StopTimer();
                }

                Logger.LogInformation($"Current Time: {streamTimerData.CurrentTime}");
            }
        }
        else
        {
            Logger.LogError($"IsRunning is {streamTimerData.TimerSettings.IsRunning}.");
        }
    }

    public void AddTime(TimeSpan addTime)
    {
        if (addTime.TotalSeconds < 0)
        {
            addTime.Multiply(-1);
        }
        streamTimerData.CurrentTime += addTime;
    }

    public void RemoveTime(TimeSpan removeTime, bool noNegative = true)
    {
        if (removeTime.TotalSeconds < 0)
        {
            removeTime.Multiply(-1);
        }
        if (noNegative == true && streamTimerData.CurrentTime - removeTime < TimeSpan.Zero)
        {
            streamTimerData.CurrentTime = TimeSpan.Zero;
            return;
        }
        else
        {
            streamTimerData.CurrentTime -= removeTime;
        }
    }

    public void StartTimer()
    {
        if (streamTimerData.TimerSettings.IsRunning == false)
        {
            streamTimerData.TimerSettings.IsRunning = true;
        }
    }

    public void StopTimer()
    {
        if (streamTimerData.TimerSettings.IsRunning == true)
        {
            streamTimerData.TimerSettings.IsRunning = false;
        }
    }

    public void SetTimerData(StreamTimerModel newTimerData)
    {
        streamTimerData = newTimerData;
    }

    public void SetStartingTime(int hours = 0, int mins = 0, int secs = 0)
    {
        var totalTime = 0;
        totalTime = (((hours * 60) * 60) * 1000) + ((mins * 60) * 1000) + (secs * 1000);
        streamTimerData.TimerSettings.StartingTime = TimeSpan.FromMilliseconds(totalTime);
    }

    public void ClearTimer()
    {
        StopTimer();
        streamTimerData.CurrentTime = TimeSpan.Zero;
        streamTimerData.TimeElapsed = TimeSpan.Zero;
    }
}
