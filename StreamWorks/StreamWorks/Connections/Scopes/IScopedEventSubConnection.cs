using StreamWorks.Library.Models.Connections.TwitchEvent;

namespace StreamWorks.Connections.Scopes;

public interface IScopedEventSubConnection
{
    Task<EventSubConnectionModel> CreateScopedEventSubConnection(CancellationToken cancellationToken, Guid loggedInUserId, string accessToken, string twitchUserId);
}
