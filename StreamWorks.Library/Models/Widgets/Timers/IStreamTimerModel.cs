namespace StreamWorks.Library.Models.Widgets.Timers;

public interface IStreamTimerModel
{
    double AddTime { get; set; }
    TimeSpan CurrentTime { get; set; }
    double DefaultTime { get; set; }
    bool FirstRun { get; set; }
    string? Id { get; set; }
    bool IsCompleted { get; set; }
    bool IsRunning { get; set; }
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

    // ==> Sub Gifts
    string TwitchSubGiftEvent { get; set; }
    int TwitchSubGiftEventCount { get; set; }
    int TwitchTotalSubGiftCount { get; set; }
    string TwitchTier1SubGiftEvent { get; set; }
    int TwitchTotalT1SubGiftCount { get; set; }
    double TwitchTier1SubGiftTime { get; set; }
    string TwitchTier2SubGiftEvent { get; set; }
    int TwitchTotalT2SubGiftCount { get; set; }
    double TwitchTier2SubGiftTime { get; set; }
    string TwitchTier3SubGiftEvent { get; set; }
    int TwitchTotalT3SubGiftCount { get; set; }
    double TwitchTier3SubGiftTime { get; set; }
    double TwitchSubGiftTime { get; set; }

    // ==> Cheers
    string TwitchCheerEvent { get; set; }
    int TwitchCheerEventCount { get; set; }
    double TwitchCheerTime { get; set; }
    int TwitchTotalCheerAmount { get; set; }

    // ==> Raids
    string TwitchRaidEvent { get; set; }
    int TwitchRaidEventCount { get; set; }
    double TwitchRaidTime { get; set; }
    int TwitchTotalRaidAmount { get; set; }

    Guid UserId { get; set; }
    string YouTubeLikeEvent { get; set; }
    int YoutubeLikeEventCount { get; set; }
    double YouTubeLikeTime { get; set; }
    string YouTubeSubEvent { get; set; }
    int YoutubeSubEventCount { get; set; }
    double YouTubeSubTime { get; set; }

    // ==> Donations
    string DonationEvent { get; set; }
    int DonationEventCount { get; set; }
    double DonationTotalAmount { get; set; }
    double DonationTime { get; set; }

    string? TimerFormat { get; set; }
}