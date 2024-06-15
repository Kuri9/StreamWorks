namespace StreamWorks.Library.Models.Widgets.Timers.TimerModels;
public class TimerYoutubeEventsModel
{
    // ==> Youtube
    // Likes
    public string YouTubeLikeEvent { get; set; } = "YoutubeLike";
    public int YoutubeLikeEventCount { get; set; }
    public int TotalYoutubeLikeEventCount { get; set; }
    public TimeSpan SetYouTubeLikeTime { get; set; } = TimeSpan.FromSeconds(300);
    public TimeSpan TotalYouTubeLikeTime { get; set; } = TimeSpan.FromSeconds(300);
    // Subscriptions
    public string YouTubeSubEvent { get; set; } = "YoutubeSubscribe";
    public int YoutubeSubEventCount { get; set; }
    public int TotalYoutubeSubEventCount { get; set; }
    public TimeSpan SetYouTubeSubTime { get; set; } = TimeSpan.FromSeconds(300);
    public TimeSpan TotalYouTubeSubTime { get; set; } = TimeSpan.FromSeconds(300);
}
