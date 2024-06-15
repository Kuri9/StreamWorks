using StreamWorks.Library.Models.Widgets.Timers.TimerModels;

namespace StreamWorks.Library.Models.Widgets.Timers;

public interface IStreamTimerModel
{
    TimeSpan AddTime { get; set; }
    TimeSpan CurrentTime { get; set; }
    TimerSettingsModel TimerSettings { get; set; }
    TimerTwitchEventsModel TwitchEvents { get; set; }
    TimerYoutubeEventsModel YoutubeEvents { get; set; }
    TimerOtherEventsModel OtherEvents { get; set; }
}