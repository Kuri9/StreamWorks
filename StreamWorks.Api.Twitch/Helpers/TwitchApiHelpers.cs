using StreamWorks.ApiLibrary.Twitch.Models.Config;

namespace StreamWorks.Api.Twitch.Helpers;

public static class TwitchApiHelpers
{
    public static Dictionary<string, string> CreateTwitchUrl(string clientId, string redirectUrl)
    {
        var result = new Dictionary<string, string>();
        var state = GetRandomString();

        TwitchScope scopesList = new TwitchScope();
        var scopesString = string.Join(" ", scopesList);
        var url = $"https://id.twitch.tv/oauth2/authorize?" +
            $"response_type=code" +
            $"&client_id={clientId}" +
            $"&redirect_uri={redirectUrl}" +
            $"&response_type=code" +
            $"&scope={scopesString}" +
            $"&state={state}";

        result.Add("url", url);
        result.Add("state", state);

        return result;
    }

    public static string GetTwitchAccessCode(string clientId, string clientSecret, string code, string redirectUrl)
    {
        TwitchScope scopesList = new TwitchScope();
        var scopesString = string.Join(" ", scopesList);
        var url = $"https://id.twitch.tv/oauth2/token?" +
            $"client_id={clientId}" +
            $"client_secret={clientSecret}" +
            $"&code={code}" +
            $"&grant_type=authorization_code" +
            $"&redirect_uri={redirectUrl}";

        return url;
    }

    public static string GetTwitchRefreshToken(string clientId, string clientSecret, string refreshToken)
    {
        var url = $"https://id.twitch.tv/oauth2/token?" +
            $"&grant_type=refresh_token" +
            $"&refresh_token={refreshToken}" +
            $"client_id={clientId}" +
            $"client_secret={clientSecret}";

        return url;
    }

    public static string GetRandomString()
    {
        var random = new Random();
        var characterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var length = 16;

        return new string(
            Enumerable.Repeat(characterSet, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
