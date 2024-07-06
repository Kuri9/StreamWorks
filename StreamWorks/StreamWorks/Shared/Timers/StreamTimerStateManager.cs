using StreamWorks.Library.Models.Widgets.Timers;

namespace StreamWorks.Shared.Timers;

public class StreamTimerStateManager
{
    private IStreamTimer _streamTimer;
    public StreamTimerModel _streamTimerData { get; private set; }

    public StreamTimerStateManager(IStreamTimer streamTimer)
    {
        _streamTimer = streamTimer;
        _streamTimerData = _streamTimer.Timer;
    }

    public event Action? OnChange;

    public void UpdateStreamTimerData(StreamTimerModel newtimerData)
    {
        _streamTimerData = newtimerData;
        NotifyTimerDataStateChanged();
    }

    private void NotifyTimerDataStateChanged() => OnChange?.Invoke();
}
