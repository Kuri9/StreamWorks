using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR;
using StreamWorks.Helpers.Widgets.Timers;
using StreamWorks.Hubs;
using StreamWorks.Library.Models.Widgets.Timers;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StreamWorks.Shared.Timers;

public class StreamTimerStateManager
{
    private ILogger<StreamTimerStateManager> Logger;
    private IConfiguration Config;
    private IStreamWorksUserData _userData;
    private AuthenticationStateProvider _authProvider;
    private StreamWorksUserModel loggedInUser = default!;
    private IHubContext<StreamHub> _hubContext { get; }

    private IStreamWorksTimerData _timerData;

    public StreamTimerModel TimerConfig { get; private set; }
    public TimeSpan CurrentTime { get; private set; }
    public TimeSpan ElapsedTime { get; private set; }
    public TimeSpan RemainingTime { get; private set; }
    public TimeSpan StartingTime { get; private set; }
    public TimeSpan MaximumTime { get; private set; }

    private TimeSpan oneSecond = TimeSpan.FromSeconds(1);

    public StreamTimerStateManager(ILogger<StreamTimerStateManager> logger, IStreamWorksUserData userData, AuthenticationStateProvider authState, IConfiguration config, IHubContext<StreamHub> hubContext, IStreamWorksTimerData timerData)
    {
        Logger = logger;
        Config = config;
        _userData = userData;
        _authProvider = authState;
        _hubContext = hubContext;
        _timerData = timerData;

        TimerConfig = new StreamTimerModel();
        TimerConfig.TimerSettings.StartingTime = TimeSpan.FromSeconds(300);
        TimerConfig.TimerSettings.IsRunning = false;

        Setup();
    }

    public event Action? OnChange;

    private async Task Setup()
    {
        loggedInUser = await _authProvider.GetUserFromAuth(_userData);

        if (loggedInUser is not null)
        {
            var result = await GetTimerConfig(loggedInUser.Id);

            if (result is null)
            {
                TimerConfig = new StreamTimerModel();
                TimerConfig.UserId = loggedInUser.Id;
                SetStartingTime(0, 5, 0);
            }
            else
            {
                SetTimerConfig(result);
            }
        }
        else
        {
            TimerConfig = new StreamTimerModel();
            SetStartingTime(0, 5, 0);
        }

        StartingTime = TimerConfig.TimerSettings.StartingTime;
        MaximumTime = TimerConfig.TimerSettings.MaximumTime;
        UpdateTimes();

        //StartTimer();
    }

    public async Task OnTimerTicked()
    {
        //await _hubContext.Clients.All.SendAsync("TimerTicked");
        //Logger.LogInformation("Timer Ticked");

        if (TimerConfig.TimerSettings.IsRunning == true)
        {
            if (TimerConfig.TimerSettings.IsCountDown == true)
            {
                TimerConfig.CurrentTime -= oneSecond;
                TimerConfig.TimeElapsed += oneSecond;

                if (TimerConfig.CurrentTime <= TimeSpan.Zero)
                {
                    StopTimer();
                }

                Logger.LogInformation($"Current Time: {TimerConfig.CurrentTime}");

                UpdateTimes();
            }
            else
            {
                TimerConfig.CurrentTime += oneSecond;
                TimerConfig.TimeElapsed += oneSecond;

                if (TimerConfig.CurrentTime >= TimeSpan.Zero)
                {
                    StopTimer();
                }

                Logger.LogInformation($"Current Time: {TimerConfig.CurrentTime}");

                UpdateTimes();
            }
        }
        else
        {
            Logger.LogError($"IsRunning is {TimerConfig.TimerSettings.IsRunning}.");
        }
    }

    public void AddTime(TimeSpan addTime)
    {
        if (addTime.TotalSeconds < 0)
        {
            addTime.Multiply(-1);
        }
        TimerConfig.CurrentTime += addTime;
    }

    public void RemoveTime(TimeSpan removeTime, bool noNegative = true)
    {
        if (removeTime.TotalSeconds < 0)
        {
            removeTime.Multiply(-1);
        }
        if (noNegative == true && TimerConfig.CurrentTime - removeTime < TimeSpan.Zero)
        {
            TimerConfig.CurrentTime = TimeSpan.Zero;
            return;
        }
        else
        {
            TimerConfig.CurrentTime -= removeTime;
        }
    }

    public void StartTimer()
    {
        if (TimerConfig.TimerSettings.IsRunning == false)
        {
            TimerConfig.TimerSettings.IsRunning = true;
        }
    }

    public void StopTimer()
    {
        if (TimerConfig.TimerSettings.IsRunning == true)
        {
            TimerConfig.TimerSettings.IsRunning = false;
        }
    }

    public void SetStartingTime(int hours = 0, int mins = 0, int secs = 0)
    {
        var totalTime = 0;
        totalTime = (((hours * 60) * 60) * 1000) + ((mins * 60) * 1000) + (secs * 1000);
        TimerConfig.TimerSettings.StartingTime = TimeSpan.FromMilliseconds(totalTime);
        NotifyStateChanged();
    }

    public void UpdateTimes()
    {
        CurrentTime = TimerConfig.CurrentTime;
        ElapsedTime = TimerConfig.TimeElapsed;

        NotifyStateChanged();
    }

    public void ClearTimer()
    {
        StopTimer();
        TimerConfig.CurrentTime = TimeSpan.Zero;
        CurrentTime = TimeSpan.Zero;
        TimerConfig.TimeElapsed = TimeSpan.Zero;
        ElapsedTime = TimeSpan.Zero;

        NotifyStateChanged();
    }

    public void SetTimerConfig(StreamTimerModel newtimerData)
    {
        TimerConfig = newtimerData;
    }

    public async Task<StreamTimerModel> GetTimerConfig(Guid id)
    {
        var result = await _timerData.GetTimerDataByUserId(id);
        return result.First();
    }

    public async Task UpdateTimerConfig()
    {
        await _timerData.UpdateTimerData(TimerConfig);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
