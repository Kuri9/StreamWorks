using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using StreamWorks.ApiLibrary.Twitch.Models.Config;
using StreamWorks.Library.Models.Users.Twitch;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace StreamWorks.Helpers.Twitch;

public static class TwitchSignInHelpers
{
    public static string GetTwitchSignInUrl(TwitchConnectionModel connectionModel)
    {
        var scopesString = string.Join(" ", connectionModel.Scopes);
        return $"https://id.twitch.tv/oauth2/authorize?client_id={connectionModel.ClientId}&redirect_uri={connectionModel.RedirectUri}&response_type=code&scope={scopesString}";
    }

    public static async Task<TwitchAccessDataModel> HandleTwitch400Error(StreamWorksUserModel user, TwitchConnectionModel connectionModel)
    {
        // Probably a better way to handle this than a Try Catch inside a Try Catch, but it works for now.
        try
        {
            var result = await RefreshTwitchToken(connectionModel);

            if (result is not null)
            {
                if (user.TwitchAccessData is not null)
                {
                    var userData = user.TwitchAccessData;
                    userData.AccessToken = result.AccessToken;
                    userData.RefreshToken = result.RefreshToken;
                    userData.ExpiresIn = result.ExpiresIn;
                    userData.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(result.ExpiresIn);

                    user.TwitchAccessData = userData;

                    return userData;
                }
                else
                {
                    throw new Exception("User Twitch Access Data is null.");
                }
            }
            else { throw new Exception("RefreshTwitchTokenReult was null"); }
        }
        catch (Exception noRefresh)
        {
            Console.WriteLine($"Failed to renew Token: {noRefresh.Message}");

            try
            {
                var result = await GetTwitchAccessDataAsync(connectionModel);
                if (result is not null)
                {
                    user.TwitchAccessData = result;

                    return result;
                }
                else { throw new Exception("GetTwitchAccessDataAsync was null"); }
            }
            catch (Exception noCode)
            {
                Console.WriteLine($"Failed to get new Token: {noCode.Message}");
            }

            Console.WriteLine("Couldn't get an Access Token. All attempts failed.");
            return null;
        }
    }

    public static async Task UpdateUserTokens(StreamWorksUserModel user, UserManager<StreamWorksUserModel> userManager, TwitchAccessDataModel twitchAccessDataModel)
    {
        try
        {
            await userManager.SetAuthenticationTokenAsync(user, "TwitchLogin", "access_token", twitchAccessDataModel.AccessToken);
            await userManager.SetAuthenticationTokenAsync(user, "TwitchLogin", "refresh_token", twitchAccessDataModel.RefreshToken);
            await userManager.SetAuthenticationTokenAsync(user, "TwitchLogin", "expires_at", twitchAccessDataModel.ExpiresAt.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to update user tokens: {ex.Message}");
        }
    }

    public static async Task<TwitchAccessDataModel> GetTwitchAccessDataAsync(TwitchConnectionModel connectionModel)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://id.twitch.tv/oauth2/token");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", connectionModel.ClientId },
            { "client_secret", connectionModel.ClientSecret },
            { "code", connectionModel.Code },
            { "grant_type", "authorization_code" },
            { "redirect_uri", connectionModel.RedirectUri }
        });

        var response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        var twitchAccessData = JsonSerializer.Deserialize<TwitchAccessDataModel>(responseContent);

        if (twitchAccessData is null)
        {
            throw new Exception("Failed to deserialize Twitch access data.");
        }

        twitchAccessData.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(twitchAccessData.ExpiresIn);

        return twitchAccessData;
    }

    public static async Task<TwitchUserDataModel> RequestUserInfoFromTwitch(string authToken)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://id.twitch.tv/oauth2/userinfo");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "content-type", "application/json" },

            { "authorization", string.Concat("Bearer ", authToken) },
        });

        var response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        var twitchUserData = JsonSerializer.Deserialize<TwitchUserDataModel>(responseContent);

        if (twitchUserData is null)
        {
            throw new Exception("Failed to deserialize Twitch user data.");
        }

        return twitchUserData;
    }

    public static async Task<TwitchAccessDataModel> RefreshTwitchToken(TwitchConnectionModel connectionModel)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://id.twitch.tv/oauth2/token");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", connectionModel.RefreshToken},
            { "client_id", connectionModel.ClientId },
            { "client_secret", connectionModel.ClientSecret }
        });

        var response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        var twitchRefreshData = JsonSerializer.Deserialize<TwitchAccessDataModel>(responseContent);

        if (twitchRefreshData is null)
        {
            throw new Exception("Failed to deserialize Twitch refresh data.");
        }

        twitchRefreshData.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(twitchRefreshData.ExpiresIn);

        return twitchRefreshData;
    }
}