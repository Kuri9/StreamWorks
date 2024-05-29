namespace StreamWorks.Models.Widgets.Timers;

public class CreateStreamTimerModel
{
    public int StartingTime { get; set; } = 300;
    public bool ShowTotalCounts { get; set; } = false;

    // Twitch
    public int TwitchFollowTime { get; set; } = 300;
    public int TwitchTier1Time { get; set; } = 300;
    public int TwitchTier2Time { get; set; } = 600;
    public int TwitchTier3Time { get; set; } = 1000;

    // Youtube
    public int YouTubeLikeTime { get; set; } = 300;
    public int YouTubeSubTime { get; set; } = 300;

}
