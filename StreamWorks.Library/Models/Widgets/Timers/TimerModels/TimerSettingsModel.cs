namespace StreamWorks.Library.Models.Widgets.Timers.TimerModels;
public class TimerSettingsModel
{
    public string? TimerTitle { get; set; }
    public TimeSpan StartingTime { get; set; }
    public TimeSpan TotalTime { get; set; }
    public TimeSpan DefaultTime { get; set; } = TimeSpan.FromSeconds(300);
    public string? TimerFormat { get; set; }
    public string? TimeNumFormat { get; set; } = @"dd\:hh\:mm\:ss";
    public string? TimeCharFormat { get; set; } = @"d'd 'h'h 'm'm 's's'";

    public string? LastSystemMessage { get; set; }
    public int TickCount { get; set; }

    public bool FirstRun { get; set; } = true;
    public bool IsRunning { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsShowRemaining { get; set; } = true;
    public bool IsShowElapsed { get; set; } = false;
    public bool IsShowTotal { get; set; } = false;

    public TimeSpan LastSetTime { get; set; }
    public bool ShowLast { get; set; } = true;
    public bool ShowTimer { get; set; } = true;


    public bool ShowTotalCounts { get; set; } = false;
}
