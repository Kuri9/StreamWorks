using StreamWorks.Library.Models.Widgets.Timers.TimerModels;

namespace StreamWorks.Library.Models.Widgets.Timers;
public class StreamTimerModel : IStreamTimerModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public Guid UserId { get; set; }
    public TimeSpan CurrentTime { get; set; }
    public TimeSpan AddTime { get; set; }

    public TimerSettingsModel TimerSettings { get; set; } = new TimerSettingsModel();
    public TimerTwitchEventsModel TwitchEvents { get; set; } = new TimerTwitchEventsModel();
    public TimerYoutubeEventsModel YoutubeEvents { get; set; } = new TimerYoutubeEventsModel();
    public TimerOtherEventsModel OtherEvents { get; set; } = new TimerOtherEventsModel();
}
