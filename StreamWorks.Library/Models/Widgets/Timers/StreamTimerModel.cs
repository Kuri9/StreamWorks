namespace StreamWorks.Library.Models.Widgets.Timers;
public class StreamTimerModel : IStreamTimerModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public Guid UserId { get; set; }
    public string? TimerTitle { get; set; }
    public double StartingTime { get; set; }
    public TimeSpan CurrentTime { get; set; }
    public TimeSpan TotalTime { get; set; }
    public double AddTime { get; set; }
    public double DefaultTime { get; set; } = 300;

    public string? LastSystemMessage { get; set; }
    public int TickCount { get; set; }

    public bool FirstRun { get; set; } = true;
    public bool IsRunning { get; set; }
    public bool IsCompleted { get; set; }

    public bool ShowTimer { get; set; } = true;
    public bool ShowTime { get; set; } = true;

    public TimeSpan LastSetTime { get; set; }
    public bool ShowLast { get; set; } = true;


    public bool ShowTotalCounts { get; set; } = false;

    // Twitch

    // ==> Follows
    public string TwitchFollowEvent { get; set; } = "TwitchFollow";
    public int TwitchFollowEventCount { get; set; }
    public int TotalTwitchFollowEventCount { get; set; }
    public double TwitchFollowTime { get; set; } = 300;

    // ==> Subs
    public string TwitchTier1Event { get; set; } = "TwitchTier1";
    public int TwitchTier1EventCount { get; set; }
    public int TotalTwitchTier1EventCount { get; set; }
    public double TwitchTier1Time { get; set; } = 300;
    public string TwitchTier2Event { get; set; } = "TwitchTier2";
    public int TwitchTier2EventCount { get; set; }
    public int TotalTwitchTier2EventCount { get; set; }
    public double TwitchTier2Time { get; set; } = 600;
    public string TwitchTier3Event { get; set; } = "TwitchTier3";
    public int TwitchTier3EventCount { get; set; }
    public int TotalTwitchTier3EventCount { get; set; }
    public double TwitchTier3Time { get; set; } = 1000;

    // ==> Sub Gifts
    public string TwitchSubGiftEvent { get; set; } = "TwitchSubGift";
    public int TwitchSubGiftEventCount { get; set; }
    public int TwitchTotalSubGiftCount { get; set; }
    public string TwitchTier1SubGiftEvent { get; set; } = "TwitchTier1SubGift";
    public int TwitchTotalT1SubGiftCount { get; set; }
    public double TwitchTier1SubGiftTime { get; set; } = 300;
    public string TwitchTier2SubGiftEvent { get; set; } = "TwitchTier2SubGift";
    public int TwitchTotalT2SubGiftCount { get; set; }
    public double TwitchTier2SubGiftTime { get; set; } = 600;
    public string TwitchTier3SubGiftEvent { get; set; } = "TwitchTier3SubGift";
    public int TwitchTotalT3SubGiftCount { get; set; }
    public double TwitchTier3SubGiftTime { get; set; } = 1000;
    public double TwitchSubGiftTime { get; set; }

    // ==> Cheers
    public string TwitchCheerEvent { get; set; } = "TwitchCheer";
    public int TwitchCheerEventCount { get; set; }
    public double TwitchCheerTime { get; set; } = 1;
    public int TwitchTotalCheerAmount { get; set; }

    // ==> Raids
    public string TwitchRaidEvent { get; set; } = "TwitchRaid";
    public int TwitchRaidEventCount { get; set; }
    public double TwitchRaidTime { get; set; } = 1;
    public int TwitchTotalRaidAmount { get; set; }

    // Youtube
    public string YouTubeLikeEvent { get; set; } = "YoutubeLike";
    public int YoutubeLikeEventCount { get; set; }
    public int TotalYoutubeLikeEventCount { get; set; }
    public double YouTubeLikeTime { get; set; } = 300;
    public string YouTubeSubEvent { get; set; } = "YoutubeSubscribe";
    public int YoutubeSubEventCount { get; set; }
    public int TotalYoutubeSubEventCount { get; set; }
    public double YouTubeSubTime { get; set; } = 300;
}
