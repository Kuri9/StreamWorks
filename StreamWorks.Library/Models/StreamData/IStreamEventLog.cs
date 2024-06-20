
namespace StreamWorks.Library.Models.StreamData;

public interface IStreamEventLog
{
    DateTimeOffset CreatedOn { get; set; }
    TwitchEventDataModel TwitchEventData { get; set; }
    Guid UserId { get; set; }
}