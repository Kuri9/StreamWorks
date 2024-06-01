namespace StreamWorks.Models.Widgets.Timers;

public class CreateStreamTimerModel
{
    public double StartingTime { get; set; } = 300;
    public bool ShowTotalCounts { get; set; } = false;

    // Twitch
    public double TwitchFollowTime { get; set; } = 300;
    public double TwitchTier1Time { get; set; } = 300;
    public double TwitchTier2Time { get; set; } = 600;
    public double TwitchTier3Time { get; set; } = 1000;


    public double TwitchTier1SubGiftTime { get; set; } = 300;
    public double TwitchTier2SubGiftTime { get; set; } = 600;
    public double TwitchTier3SubGiftTime { get; set; } = 1000;

    // ==> Cheers
    public double TwitchCheerTime { get; set; } = 1;

    // Youtube
    public double YouTubeLikeTime { get; set; } = 300;
    public double YouTubeSubTime { get; set; } = 300;

}
