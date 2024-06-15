namespace StreamWorks.Library.Models.StreamData;
public class StreamEventLogModel
{
    public string? UserId { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    public TwitchEventDataModel TwitchEventData { get; set; } = new TwitchEventDataModel();
}
