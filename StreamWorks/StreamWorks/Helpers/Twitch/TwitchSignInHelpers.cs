using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using StreamWorks.ApiLibrary.Twitch.Models.Config;
using StreamWorks.Library.Models.Users.Twitch;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TwitchLib.Api;

namespace StreamWorks.Helpers.Twitch;

public class TwitchSignInHelpers : ITwitchSignInHelpers
{
    private readonly IConfiguration _config;
    private readonly TwitchAPI _twitchApi;
    private readonly UserManager<StreamWorksUserModel> _userManager;
    public TwitchSignInHelpers(IConfiguration config, UserManager<StreamWorksUserModel> userManager, TwitchAPI twitchApi)
    {
        _config = config;
        _userManager = userManager;
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

    public async Task RefreshTwitchToken(StreamWorksUserModel user, string refreshToken)
    {
        var refreshedToken = await _twitchApi.Auth.RefreshAuthTokenAsync(refreshToken, _twitchApi.Settings.Secret);

        if (refreshedToken is not null)
        {
            try
            {
                var expiresAt = DateTimeOffset.Now.AddSeconds(refreshedToken.ExpiresIn).ToString();

                await _userManager.SetAuthenticationTokenAsync(user, "TwitchLogin", "access_token", refreshedToken.AccessToken);
                await _userManager.SetAuthenticationTokenAsync(user, "TwitchLogin", "refresh_token", refreshedToken.RefreshToken);
                await _userManager.SetAuthenticationTokenAsync(user, "TwitchLogin", "expires_at", expiresAt);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update user tokens: {ex.Message}");
            }
        }
    }
}