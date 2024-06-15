namespace StreamWorks.Models.Widgets.Timers;

public class TimerDataModel
{
    public TimerDataModel()
    {
        TimerId = Guid.NewGuid();
    }

    public Guid TimerId { get; set; }
    public System.Timers.Timer? Timer { get; set; }

    public TimeSpan CurrentTime { get; set; }
    public TimeSpan Duration { get; set; }
    public TimeSpan TimeElapsed { get; set; }

    public int TimerTicks { get; set; } = 1000;

    public void GetTimeElapsed()
        {
            TimeElapsed = Duration - CurrentTime;
        }
}
