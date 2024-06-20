
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
}