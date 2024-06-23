using StreamWorks.Library.DataAccess.MongoDB.StreamWorks.StreamEventsData;

namespace StreamWorks.Library.Models.StreamData;
public class StreamEventLogModel : IStreamEventLog
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    public TwitchEventDataModel TwitchEventData { get; set; } = new TwitchEventDataModel();
}
