
namespace StreamWorks.Library.Models.Users.Twitch.Widgets.Timers;

public interface IStreamTimerModel
{
    double AddTime { get; set; }
    TimeSpan CurrentTime { get; set; }
    bool FirstRun { get; set; }
    string Id { get; set; }
    bool IsCompleted { get; set; }
    bool IsPaused { get; set; }
    bool IsRunning { get; set; }
    bool IsStopped { get; set; }
    TimeSpan LastSetTime { get; set; }
    string? LastSystemMessage { get; set; }
    bool ShowLast { get; set; }
    bool ShowTime { get; set; }
    bool ShowTimer { get; set; }
    bool ShowTotalCounts { get; set; }
    double StartingTime { get; set; }
    int TickCount { get; set; }
    string? TimerTitle { get; set; }
    TimeSpan TotalTime { get; set; }
    int TotalTwitchFollowEventCount { get; set; }
    int TotalTwitchTier1EventCount { get; set; }
    int TotalTwitchTier2EventCount { get; set; }
    int TotalTwitchTier3EventCount { get; set; }
    int TotalYoutubeLikeEventCount { get; set; }
    int TotalYoutubeSubEventCount { get; set; }
    string TwitchFollowEvent { get; set; }
    int TwitchFollowEventCount { get; set; }
    double TwitchFollowTime { get; set; }
    string TwitchTier1Event { get; set; }
    int TwitchTier1EventCount { get; set; }
    double TwitchTier1Time { get; set; }
    string TwitchTier2Event { get; set; }
    int TwitchTier2EventCount { get; set; }
    double TwitchTier2Time { get; set; }
    string TwitchTier3Event { get; set; }
    int TwitchTier3EventCount { get; set; }
    double TwitchTier3Time { get; set; }
    Guid UserId { get; set; }
    string YouTubeLikeEvent { get; set; }
    int YoutubeLikeEventCount { get; set; }
    double YouTubeLikeTime { get; set; }
    string YouTubeSubEvent { get; set; }
    int YoutubeSubEventCount { get; set; }
    double YouTubeSubTime { get; set; }
}