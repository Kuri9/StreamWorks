using StreamWorks.Library.Models.TwitchApi.Config;
using StreamWorks.Library.Models.TwitchApi.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamWorks.Library.Models.Users.UserData;
public class UserAppStateModel : IUserAppState
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public Guid UserId { get; set; } = Guid.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset LastLogin { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset LastLogout { get; set; } = DateTimeOffset.Now;
    public bool IsLoggedIn { get; set; } = false;
    public bool IsStreaming { get; set; } = false;
    public StreamEventLogModel EventLogs { get; set; } = new StreamEventLogModel();
    public bool TwitchAccountConnected { get; set; } = false;
    public TwitchConnectionModel TwitchConnection { get; set; } = new TwitchConnectionModel();
    public GetUserDataModel TwitchUserData { get; set; } = new GetUserDataModel();
}
