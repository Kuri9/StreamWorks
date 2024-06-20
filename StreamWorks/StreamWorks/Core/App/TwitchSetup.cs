using Azure.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using StreamWorks.Hubs;
using StreamWorks.Library.Models.TwitchApi.Config;
using StreamWorks.Library.Models.Users.UserData;
using TwitchLib.Api;
using TwitchLib.Api.Core.Extensions.System;
using TwitchLib.Api.Helix;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StreamWorks.Core.App;

public class TwitchSetup
{
    private ILogger<TwitchSetup> Logger;
    private IConfiguration Config;
    private IUserAppState AppState;
    private TwitchAPI twitchApi;
    private UserManager<StreamWorksUserModel> UserManager;
    private NavigationManager NavManager;

    private TwitchConnectionModel twitchConnectionData = new TwitchConnectionModel();
    private List<GetUserDataModel>? twitchUserData = new();

    private bool mustRefreshToken = false;

    public TwitchSetup(ILogger<TwitchSetup> _logger, IConfiguration config, IUserAppState appState, UserManager<StreamWorksUserModel> userManager, NavigationManager navManager, TwitchAPI twitchApi)
    {
        Logger = _logger;
        Config = config;
        AppState = appState;
        UserManager = userManager;
        NavManager = navManager;
        this.twitchApi = twitchApi;
    }

    public TwitchConnectionModel SetupTwitchUser(StreamWorksUserModel loggedInUser)
    {
        if (loggedInUser is not null)
        {
            if (loggedInUser.Logins.Where(l => l.LoginProvider == "TwitchLogin").Count() > 0)
            {
                Logger.LogInformation($"Setting Twitch Connection");

                var connectionData = new TwitchConnectionModel();

                connectionData = SetTwitchConnectionData(loggedInUser, connectionData);
                connectionData = SetTwitchId(loggedInUser, connectionData);
                connectionData = GetAccessTokenFromUser(loggedInUser, connectionData);

                return connectionData;
            }
            else
            {
                Logger.LogError("User does not have a TwitchLogin");
                return null;
            }
        }
        else
        {
            Logger.LogError("User is null!");
            return null;
        }
    }

    public async Task RefreshTwitchToken(UserAppStateModel appState, StreamWorksUserModel loggedInUser)
    {
        Logger.LogInformation($"Starting Refresh Token...");
        var refreshToken = appState.TwitchConnection.RefreshToken;
        this.twitchApi.Settings.ClientId = appState.TwitchConnection.ClientId;
        this.twitchApi.Settings.Secret = appState.TwitchConnection.ClientSecret;

        var refreshedToken = await this.twitchApi.Auth.RefreshAuthTokenAsync(
                appState.TwitchConnection.RefreshToken,
                appState.TwitchConnection.ClientId,
                appState.TwitchConnection.ClientSecret
            );

        if (refreshedToken is not null)
        {
            Logger.LogInformation($"Refresh Token Response not null!");
            try
            {
                var expiresAt = DateTimeOffset.Now.AddSeconds(refreshedToken.ExpiresIn).ToString();
                Logger.LogInformation($"Received Token: {refreshedToken.AccessToken}");

                await UserManager.SetAuthenticationTokenAsync(loggedInUser, "TwitchLogin", "access_token", refreshedToken.AccessToken);
                await UserManager.SetAuthenticationTokenAsync(loggedInUser, "TwitchLogin", "refresh_token", refreshedToken.RefreshToken);
                await UserManager.SetAuthenticationTokenAsync(loggedInUser, "TwitchLogin", "expires_at", expiresAt);

                Logger.LogInformation($"Current LoggedInUser Token: {loggedInUser?.Tokens.Where(t => t.Name == "access_token").First().Value}");

                this.twitchApi.Settings.AccessToken = refreshedToken.AccessToken;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to update user tokens: {ex.Message}");
            }
        }
    }

    private TwitchConnectionModel GetAccessTokenFromUser(StreamWorksUserModel loggedInUser, TwitchConnectionModel twitchConnectionData)
    {
        if (loggedInUser.Tokens is not null)
        {
            twitchConnectionData.AccessToken = loggedInUser.Tokens.Where(t => t.Name == "access_token").First().Value;
            Logger.LogInformation($"After Set User, {loggedInUser.UserName}'s Access Token: {twitchConnectionData.AccessToken}");

            return twitchConnectionData;
        }
        else
        {
            Logger.LogError("User does not have an Access Token");
            return null;
        }
    }

    private TwitchConnectionModel SetTwitchId(StreamWorksUserModel loggedInUser, TwitchConnectionModel twitchConnectionData)
    {
        twitchConnectionData.TwitchId = loggedInUser.Logins.Where(l => l.LoginProvider == "TwitchLogin").First().ProviderKey;
        Logger.LogInformation($"User Twitch ID is: {twitchConnectionData.TwitchId}");
        return twitchConnectionData;
    }

    private async Task<TwitchConnectionModel> GetBroadcasterId(string twitchName, TwitchConnectionModel twitchConnectionData)
    {
        var broadcasterList = new List<string>();
        broadcasterList.Add(twitchName);

        var result = await twitchApi.Helix.Users.GetUsersAsync(broadcasterList);
        var broadcasterId = result.Users[0].Id;
        twitchConnectionData.BroadcasterId = broadcasterId;

        if (String.IsNullOrEmpty(broadcasterId))
        {
            broadcasterId = Config["Twitch:DefaultBroadcaster"] ?? "No default broadcaster set.";
            Logger.LogInformation($"Twitch BroadcasterID was not found. Using default instead: {broadcasterId}");

            twitchConnectionData.BroadcasterId = broadcasterId;
            return twitchConnectionData;
        }
        else
        {
            Logger.LogInformation($"Twitch BroadcasterID is: {broadcasterId}");
            return twitchConnectionData;
        }
    }

    public TwitchConnectionModel SetTwitchConnectionData(StreamWorksUserModel loggedInUser, TwitchConnectionModel twitchConnectionData)
    {
        var refreshToken = loggedInUser?.GetToken("TwitchLogin", "refresh_token").Value;
        Logger.LogInformation($"Refresh Token Check and Set: {refreshToken}");

        if (refreshToken is not null)
        {
            twitchConnectionData.ClientId = Config["Twitch:ClientId"];
            Logger.LogInformation($"Client ID: {twitchConnectionData.ClientId}");

            twitchConnectionData.ClientSecret = Config["Twitch:ClientSecret"];
            Logger.LogInformation($"Client Secret: {twitchConnectionData.ClientSecret}");

            twitchConnectionData.RedirectUri = NavManager.Uri.ToString();
            Logger.LogInformation($"Redirect URI: {twitchConnectionData.RedirectUri}");

            twitchConnectionData.Scopes = new TwitchScope();

            twitchConnectionData.RefreshToken = refreshToken;
            Logger.LogInformation($"Refresh Token: {twitchConnectionData.RefreshToken}");

            return twitchConnectionData;
        }
        else
        {
            Logger.LogError($"Refresh Token was null!");
            return null;
        };
    }

    public async Task<bool> GetTwitchUserData(UserAppStateModel appState, StreamWorksUserModel loggedInUser)
    {
        Logger.LogInformation("Getting Twitch User Data...");

        if (String.IsNullOrWhiteSpace(appState.TwitchConnection.TwitchId))
        {
            appState.TwitchConnection.TwitchId = loggedInUser.Logins.Where(l => l.LoginProvider == "TwitchLogin").ToList().First().ProviderKey;
        }

        //getUserResponse = await twitchInternalUserApi.GetTwitchUserData(name, accessToken);
        var getUserResponse = await GetTwitchUserDataFromApi(appState);

        await Task.Delay(1000);

        if (getUserResponse is not null)
        {
            Logger.LogInformation($"Response not null!");

            if (getUserResponse.Users.Count() > 0)
            {
                Logger.LogInformation($"User Count is greater than 0!");

                foreach (var user in getUserResponse.Users)
                {
                    var userData = new GetUserDataModel();

                    userData.DisplayName = user.DisplayName;
                    userData.Id = user.Id;
                    userData.BroadcasterType = user.BroadcasterType;
                    userData.Description = user.Description;
                    userData.ProfileImageUrl = user.ProfileImageUrl;
                    userData.OfflineImageUrl = user.OfflineImageUrl;
                    userData.CreatedAt = user.CreatedAt;
                    userData.ViewCount = user.ViewCount;

                    twitchUserData?.Add(userData);
                }
                appState.TwitchUserData = twitchUserData.Where(t => t.Id == appState.TwitchConnection.TwitchId).First();

                Logger.LogInformation($"User Data: {twitchUserData?.First().DisplayName}");

                return true;
            }
            else
            {
                Logger.LogError("Result didn't contain any Users");
                return false;
            }
        }
        else
        {
            mustRefreshToken = true;
            Logger.LogError("Failed to get User Data. Refreshing token!");
            return false;
        }
    }

    //public async Task TwitchApiSetup()
    //{
    //    // Call API as a test
    //    if (AccessToken is not null)
    //    {
    //        // Setup EventSub with data after testing
    //        Logger.LogInformation("Setting up EventSub");

    //        if (twitchHub is null)
    //        {
    //            Logger.LogError("Twitch Hub is null! Will not call twitchHub.");
    //            return;
    //        };
    //        // await twitchHub.SendAsync("SetupConnectionRequest", AccessToken, UserId, BroadcasterId);
    //        // await twitchHub.SendAsync("StartServiceRequest");

    //        // var user = loggedInUserAuthState?.Claims.Where(c => c.Type == "user_id").First().Value;
    //        await twitchHub.SendAsync("SetupConnectionRequest", loggedInUser.Id, AccessToken, UserId);
    //    }
    //    else
    //    {
    //        Logger.LogWarning($"Access Token was null!");
    //    }
    //}

    public async Task<GetUsersResponse> GetTwitchUserDataFromApi(UserAppStateModel appState)
    {
        // var logins =  new List<string>();
        // logins.Add(id);

        try
        {
            this.twitchApi.Settings.AccessToken = appState.TwitchConnection.AccessToken;
            this.twitchApi.Settings.ClientId = appState.TwitchConnection.ClientId;
            this.twitchApi.Settings.Secret = appState.TwitchConnection.ClientSecret;

            var user = await this.twitchApi.Helix.Users.GetUsersAsync();
            return user;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error getting user data from Twitch. Error Code is: {ex.HResult}");
        }

        return null;
    }
}
