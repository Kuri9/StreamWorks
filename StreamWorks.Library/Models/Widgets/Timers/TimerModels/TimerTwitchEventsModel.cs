namespace StreamWorks.Library.Models.Widgets.Timers.TimerModels;
public class TimerTwitchEventsModel
{
    // Twitch

    // ==> Follows
    public string TwitchFollowEvent { get; set; } = "TwitchFollow";
    public int TwitchFollowEventCount { get; set; }
    public int TotalTwitchFollowEventCount { get; set; }
    public TimeSpan SetTwitchFollowTime { get; set; } = TimeSpan.FromSeconds(300);
    public TimeSpan TotalTwitchFollowTime { get; set; } = TimeSpan.Zero;

    // ==> Subs
    //Tier 1
    public string TwitchTier1Event { get; set; } = "TwitchTier1";
    public int TwitchTier1EventCount { get; set; }
    public int TotalTwitchTier1EventCount { get; set; }
    public TimeSpan SetTwitchTier1Time { get; set; } = TimeSpan.FromSeconds(300);
    public TimeSpan TotalTwitchTier1Time { get; set; } = TimeSpan.Zero;
    // Tier 2
    public string TwitchTier2Event { get; set; } = "TwitchTier2";
    public int TwitchTier2EventCount { get; set; }
    public int TotalTwitchTier2EventCount { get; set; }
    public TimeSpan SetTwitchTier2Time { get; set; } = TimeSpan.FromSeconds(600);
    public TimeSpan TotalTwitchTier2Time { get; set; } = TimeSpan.Zero;
    // Tier 3
    public string TwitchTier3Event { get; set; } = "TwitchTier3";
    public int TwitchTier3EventCount { get; set; }
    public int TotalTwitchTier3EventCount { get; set; }
    public TimeSpan SetTwitchTier3Time { get; set; } = TimeSpan.FromSeconds(1000);
    public TimeSpan TotalTwitchTier3Time { get; set; } = TimeSpan.Zero;

    // ==> Sub Gifts
    public string TwitchSubGiftEvent { get; set; } = "TwitchSubGift";
    public int TwitchSubGiftEventCount { get; set; }
    public int TwitchTotalSubGiftCount { get; set; }
    public TimeSpan TotalTwitchSubGiftTime { get; set; } = TimeSpan.Zero;
    // Tier 1
    public string TwitchTier1SubGiftEvent { get; set; } = "TwitchTier1SubGift";
    public int TwitchTotalT1SubGiftCount { get; set; }
    public TimeSpan SetTwitchTier1SubGiftTime { get; set; } = TimeSpan.FromSeconds(300);
    public TimeSpan TotalTwitchTier1SubGiftTime { get; set; } = TimeSpan.Zero;
    // Tier 2
    public string TwitchTier2SubGiftEvent { get; set; } = "TwitchTier2SubGift";
    public int TwitchTotalT2SubGiftCount { get; set; }
    public TimeSpan SetTwitchTier2SubGiftTime { get; set; } = TimeSpan.FromSeconds(600);
    public TimeSpan TotalTwitchTier2SubGiftTime { get; set; } = TimeSpan.Zero;
    // Tier 3
    public string TwitchTier3SubGiftEvent { get; set; } = "TwitchTier3SubGift";
    public int TwitchTotalT3SubGiftCount { get; set; }
    public TimeSpan SetTwitchTier3SubGiftTime { get; set; } = TimeSpan.FromSeconds(1000);
    public TimeSpan TotalTwitchTier3SubGiftTime { get; set; } = TimeSpan.Zero;

    // ==> Cheers
    public string TwitchCheerEvent { get; set; } = "TwitchCheer";
    public int TwitchCheerEventCount { get; set; }
    public int TwitchTotalCheerAmount { get; set; }
    public TimeSpan SetTwitchCheerTime { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan TotalTwitchCheerTime { get; set; } = TimeSpan.Zero;

    // ==> Raids
    public string TwitchRaidEvent { get; set; } = "TwitchRaid";
    public int TwitchRaidEventCount { get; set; }
    public int TwitchTotalRaidAmount { get; set; }
    public TimeSpan SetTwitchRaidTime { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan TotalTwitchRaidTime { get; set; } = TimeSpan.Zero;
}
