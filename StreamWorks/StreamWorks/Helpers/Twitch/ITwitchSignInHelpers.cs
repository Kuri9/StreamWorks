namespace StreamWorks.Helpers.Twitch;

public interface ITwitchSignInHelpers
{
    string GetTwitchSignInUrl(TwitchConnectionModel connectionModel);
    Task RefreshTwitchToken(string refreshToken);
}