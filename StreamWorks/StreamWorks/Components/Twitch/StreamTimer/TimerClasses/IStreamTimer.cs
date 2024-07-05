using StreamWorks.Library.Models.Widgets.Timers.TimerModels;
using StreamWorks.Models.Widgets.Timers;

namespace StreamWorks.Components.Twitch.StreamTimer.TimerClasses;
public interface IStreamTimer
{
    TimerSettingsModel Timer { get; }

    Task OnTimerTicked();
    void AddTime(TimeSpan addTime);
    void RemoveTime(TimeSpan removeTime, bool noNegative = true);
    void SetStartingTime(int hours = 0, int mins = 0, int secs = 0);
    void ClearTimer();
    void StartTimer();
    void StopTimer();
}