
namespace StreamWorks.Library.Models.StreamData;

public interface IStreamEventLog
{
    string Id { get; set; }
    DateTimeOffset CreatedOn { get; set; }
    TwitchEventDataModel TwitchEventData { get; set; }
    Guid UserId { get; set; }
}