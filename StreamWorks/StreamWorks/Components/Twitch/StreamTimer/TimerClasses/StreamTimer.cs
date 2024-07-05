using Microsoft.AspNetCore.SignalR;
using StreamWorks.Hubs;
using StreamWorks.Library.Models.Connections.TwitchEvent;
using StreamWorks.Library.Models.Widgets.Timers.TimerModels;
using StreamWorks.Models.Widgets.Timers;
using System.Numerics;
using System.Runtime.CompilerServices;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace StreamWorks.Components.Twitch.StreamTimer.TimerClasses;

public class StreamTimer : IStreamTimer
{
    private StreamWorksUserModel loggedInUser = default!;

    ILogger<StreamTimer> Logger;
    IConfiguration Config;

    private TimerDataModel timerData = new TimerDataModel();
    private TimerSettingsModel timer = new TimerSettingsModel();

    private TimeSpan oneSecond = TimeSpan.FromSeconds(1);
    public TimerSettingsModel Timer => timer;

    //public event EventHandler<TimeSpan>? TimerTickEvent;
    private IHubContext<StreamHub> _hubContext { get; }

    public StreamTimer(ILogger<StreamTimer> logger, IConfiguration config, IHubContext<StreamHub> hubContext)
    {
        Logger = logger;
        Config = config;
        _hubContext = hubContext;

        timer.StartingTime = TimeSpan.FromSeconds(300);
        timer.IsRunning = false;
    }

    public async Task OnTimerTicked()
    {
        //await _hubContext.Clients.All.SendAsync("TimerTicked");
        //Logger.LogInformation("Timer Ticked");

        if (timer.IsRunning == true)
        {
            if (timer.IsCountDown == true)
            {
                timer.CurrentTime -= oneSecond;
                timer.TimeElapsed += oneSecond;

                if (timer.CurrentTime <= TimeSpan.Zero)
                {
                    StopTimer();
                }

                Logger.LogInformation($"Current Time: {timer.CurrentTime}");
            }
            else
            {
                timer.CurrentTime += oneSecond;
                timer.TimeElapsed += oneSecond;

                if (timer.CurrentTime >= TimeSpan.Zero)
                {
                    StopTimer();
                }

                Logger.LogInformation($"Current Time: {timer.CurrentTime}");
            }
        }
        else
        {
            Logger.LogError($"IsRunning is {timer.IsRunning}.");
        }
    }

    public void AddTime(TimeSpan addTime)
    {
        if (addTime.TotalSeconds < 0)
        {
            addTime.Multiply(-1);
        }
        timer.CurrentTime += addTime;
    }

    public void RemoveTime(TimeSpan removeTime, bool noNegative = true)
    {
        if (removeTime.TotalSeconds < 0)
        {
            removeTime.Multiply(-1);
        }
        if (noNegative == true && timer.CurrentTime - removeTime < TimeSpan.Zero)
        {
            timer.CurrentTime = TimeSpan.Zero;
            return;
        }
        else
        {
            timer.CurrentTime -= removeTime;
        }
    }

    public void StartTimer()
    {
        timer.CurrentTime = timer.StartingTime;

        if (timer.IsRunning == false)
        {
            timer.IsRunning = true;
        }
    }

    public void StopTimer()
    {
        if (timer.IsRunning == true)
        {
            timer.IsRunning = false;
        }
    }

    public void SetStartingTime(int hours = 0, int mins = 0, int secs = 0)
    {
        var totalTime = 0;
        totalTime = (((hours * 60) * 60) * 1000) + ((mins * 60) * 1000) + (secs * 1000);
        timer.StartingTime = TimeSpan.FromMilliseconds(totalTime);
    }

    public void ClearTimer()
    {
        StopTimer();
        timer.CurrentTime = TimeSpan.Zero;
        timer.TimeElapsed = TimeSpan.Zero;
    }


}
