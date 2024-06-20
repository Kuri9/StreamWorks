namespace StreamWorks.Library.Models.StreamData;
public class StreamEventLogModel : IStreamEventLog
{
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    public TwitchEventDataModel TwitchEventData { get; set; } = new TwitchEventDataModel();
}
