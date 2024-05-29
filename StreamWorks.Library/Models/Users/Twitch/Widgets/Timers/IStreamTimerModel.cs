namespace StreamWorks.Library.Models.Users.Twitch.Widgets.Timers;

public interface IStreamTimerModel
{
    int AddTime { get; set; }
    int CurrentTime { get; set; }
    bool FirstRun { get; set; }
    string Id { get; set; }
    string? LastSystemMessage { get; set; }
    bool ShowTotalCounts { get; set; }
    int StartingTime { get; set; }
    int TickCount { get; set; }
    string TimerTitle { get; set; }
    int TotalTwitchFollowEventCount { get; set; }
    int TotalTwitchTier1EventCount { get; set; }
    int TotalTwitchTier2EventCount { get; set; }
    int TotalTwitchTier3EventCount { get; set; }
    int TotalYoutubeLikeEventCount { get; set; }
    int TotalYoutubeSubEventCount { get; set; }
    string TwitchFollowEvent { get; set; }
    int TwitchFollowEventCount { get; set; }
    int TwitchFollowTime { get; set; }
    string TwitchTier1Event { get; set; }
    int TwitchTier1EventCount { get; set; }
    int TwitchTier1Time { get; set; }
    string TwitchTier2Event { get; set; }
    int TwitchTier2EventCount { get; set; }
    int TwitchTier2Time { get; set; }
    string TwitchTier3Event { get; set; }
    int TwitchTier3EventCount { get; set; }
    int TwitchTier3Time { get; set; }
    Guid UserId { get; set; }
    string YouTubeLikeEvent { get; set; }
    int YoutubeLikeEventCount { get; set; }
    int YouTubeLikeTime { get; set; }
    string YouTubeSubEvent { get; set; }
    int YoutubeSubEventCount { get; set; }
    int YouTubeSubTime { get; set; }
}