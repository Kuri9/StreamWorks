
using StreamWorks.Library.Models.TwitchApi.Config;
using StreamWorks.Library.Models.TwitchApi.Users;

namespace StreamWorks.Library.Models.Users.UserData;

public interface IUserAppState
{
    DateTimeOffset CreatedOn { get; set; }
    string DisplayName { get; set; }
    bool IsLoggedIn { get; set; }
    bool IsStreaming { get; set; }
    DateTimeOffset LastLogin { get; set; }
    DateTimeOffset LastLogout { get; set; }
    Guid UserId { get; set; }
    string Id { get; set; }
    bool TwitchAccountConnected { get; set; }
    TwitchConnectionModel TwitchConnection { get; set; }
    GetUserDataModel TwitchUserData { get; set; }
}