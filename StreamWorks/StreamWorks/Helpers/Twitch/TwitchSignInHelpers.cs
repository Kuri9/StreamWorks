using Microsoft.AspNetCore.Components.Authorization;

namespace StreamWorks.Helpers.Twitch;

public class TwitchSignInHelpers : ITwitchSignInHelpers
{
    private readonly IConfiguration _config;
    private readonly TwitchAPI _twitchApi;
    private readonly UserManager<StreamWorksUserModel> _userManager;
    private readonly AuthenticationStateProvider _authProvider;
    private readonly IStreamWorksUserData UserData;
    private StreamWorksUserModel loggedInUser = new();

    public TwitchSignInHelpers(IConfiguration config, UserManager<StreamWorksUserModel> userManager, AuthenticationStateProvider authProvider, IStreamWorksUserData userData, TwitchAPI twitchApi)
    {
        _config = config;
        _userManager = userManager;
        _authProvider = authProvider;
        UserData = userData;

        _twitchApi = twitchApi;
        _twitchApi.Settings.ClientId = _config["Twitch:ClientId"];
        _twitchApi.Settings.Secret = _config["Twitch:ClientSecret"];
    }

    public string GetTwitchSignInUrl(TwitchConnectionModel connectionModel)
    {
        TwitchScope scopesList = new TwitchScope();
        var scopesString = string.Join(" ", scopesList);
        return $"https://id.twitch.tv/oauth2/authorize?client_id={connectionModel.ClientId}&redirect_uri={connectionModel.RedirectUri}&response_type=code&scope={scopesString}";
    }

    public async Task RefreshTwitchToken(string refreshToken)
    {
        if (_authProvider is not null)
        {
            Console.WriteLine($"Getting logged in User...");
            loggedInUser = await _authProvider.GetUserFromAuth(UserData);
            Console.WriteLine($"Logged in User: {loggedInUser.UserName}");
        }

        Console.WriteLine($"Refreshing Token....");
        var refreshedToken = await _twitchApi.Auth.RefreshAuthTokenAsync(refreshToken, _twitchApi.Settings.Secret);

        if (refreshedToken is not null)
        {
            Console.WriteLine($"Response not null!");
            try
            {
                var expiresAt = DateTimeOffset.Now.AddSeconds(refreshedToken.ExpiresIn).ToString();

                Console.WriteLine($"Setting New Access Token: {refreshedToken.AccessToken}");

                await _userManager.SetAuthenticationTokenAsync(loggedInUser, "TwitchLogin", "access_token", refreshedToken.AccessToken);
                await _userManager.SetAuthenticationTokenAsync(loggedInUser, "TwitchLogin", "refresh_token", refreshedToken.RefreshToken);
                await _userManager.SetAuthenticationTokenAsync(loggedInUser, "TwitchLogin", "expires_at", expiresAt);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update user tokens: {ex.Message}");
            }
        }
    }
}