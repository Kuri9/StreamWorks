namespace StreamWorks.Library.Models.Users.Twitch.Widgets.Timers;
public class StreamTimerModel : IStreamTimerModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public Guid UserId { get; set; }
    public string TimerTitle { get; set; }
    public int StartingTime { get; set; } = 300;
    public int CurrentTime { get; set; } = 0;
    public string? LastSystemMessage { get; set; }
    public int TickCount { get; set; }
    public bool FirstRun { get; set; } = true;
    public int AddTime { get; set; }

    public bool ShowTotalCounts { get; set; } = false;

    // Twitch
    public string TwitchFollowEvent { get; set; } = "TwitchFollow";
    public int TwitchFollowEventCount { get; set; }
    public int TotalTwitchFollowEventCount { get; set; }
    public int TwitchFollowTime { get; set; } = 300;
    public string TwitchTier1Event { get; set; } = "TwitchTier1";
    public int TwitchTier1EventCount { get; set; }
    public int TotalTwitchTier1EventCount { get; set; }
    public int TwitchTier1Time { get; set; } = 300;
    public string TwitchTier2Event { get; set; } = "TwitchTier2";
    public int TwitchTier2EventCount { get; set; }
    public int TotalTwitchTier2EventCount { get; set; }
    public int TwitchTier2Time { get; set; } = 600;
    public string TwitchTier3Event { get; set; } = "TwitchTier3";
    public int TwitchTier3EventCount { get; set; }
    public int TotalTwitchTier3EventCount { get; set; }
    public int TwitchTier3Time { get; set; } = 1000;

    // Youtube
    public string YouTubeLikeEvent { get; set; } = "YoutubeLike";
    public int YoutubeLikeEventCount { get; set; }
    public int TotalYoutubeLikeEventCount { get; set; }
    public int YouTubeLikeTime { get; set; } = 300;
    public string YouTubeSubEvent { get; set; } = "YoutubeSubscribe";
    public int YoutubeSubEventCount { get; set; }
    public int TotalYoutubeSubEventCount { get; set; }
    public int YouTubeSubTime { get; set; } = 300;
}
