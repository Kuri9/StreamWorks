using StreamWorks.Models.Widgets.Timers;

namespace StreamWorks.Components.Twitch.StreamTimer.TimerClasses;

public class StreamTimer : IDisposable
{
    ILogger<StreamTimer>? Logger;

    private System.Timers.Timer? _timer = new();
    private TimerDataModel timer = new TimerDataModel();

    private TimeSpan oneSecond = TimeSpan.FromSeconds(1);
    public TimerDataModel Timer => timer;
    public TimeSpan CurrentTime => timer.CurrentTime;

    //public event EventHandler<TimeSpan>? TimerTickEvent;

    public event Action? OnTimerTick;
    public event Action? OnTimerStart;
    public event Action? OnTimerStop;
    public event Action? OnTimerReset;
    public event Action? OnAddTime;
    public event Action? OnRemoveTime;

    public StreamTimer()
    {
        // Set Timer
        _timer = new System.Timers.Timer(timer.TimerTicks);
        _timer.Elapsed += OnTickEvent;
        _timer.AutoReset = true;

        timer.Timer = _timer;
        timer.CurrentTime = TimeSpan.FromSeconds(10000);
    }

    public void AddTimeSeconds(TimeSpan addTime)
    {
        if (addTime.TotalSeconds < 0)
        {
            addTime.Multiply(-1);
        }
        timer.CurrentTime += addTime;
        OnAddTime?.Invoke();
    }

    public void RemoveTimeSeconds(TimeSpan removeTime, bool noNegative = true)
    {
        if (removeTime.TotalSeconds < 0)
        {
            removeTime.Multiply(-1);
        }
        if (noNegative == true && timer.CurrentTime - removeTime < TimeSpan.Zero)
        {
            timer.CurrentTime = TimeSpan.Zero;
            OnRemoveTime?.Invoke();
            return;
        }
        else
        {
            timer.CurrentTime -= removeTime;
            OnRemoveTime?.Invoke();
        }
    }

    private void OnTickEvent(object? sender, ElapsedEventArgs e)
    {
        if (_timer is not null)
        {
            Logger.LogInformation("Timer ticked in Class");
            timer.CurrentTime -= oneSecond;
            timer.TimeElapsed += oneSecond;
            OnTimerTick?.Invoke();

            if (timer.CurrentTime <= TimeSpan.Zero)
            {
                StopTimer();
            }
        }
        else
        {
            Logger.LogError("Timer is null. Cannot Tick");
        }
    }

    public void StartTimer()
    {
        if (_timer is not null)
        {
            _timer.Start();
            OnTimerStart?.Invoke();
        }
        else
        {
            Logger.LogError("Timer is null. Cannot Start");
        }
    }

    public void StopTimer()
    {
        if (_timer is not null)
        {
            _timer.Stop();
            OnTimerStop?.Invoke();
        }
        else
        {
            Logger.LogError("Timer is null. Cannot Stop");
        }
    }

    public void ResetTimer()
    {
        if (_timer is not null)
        {
            _timer.Stop();
            timer.CurrentTime = TimeSpan.Zero;
            timer.TimeElapsed = TimeSpan.Zero;
            OnTimerReset?.Invoke();
        }
        else
        {
            Logger.LogError("Timer is null. Cannot Reset");
        }
    }



    void IDisposable.Dispose()
    {
        if (_timer is not null)
        {
            _timer.Dispose();
        }
    }
}
