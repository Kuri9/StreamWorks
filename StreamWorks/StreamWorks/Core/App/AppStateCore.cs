using Azure.Core;
using Microsoft.AspNetCore.Components.Authorization;
using StreamWorks.Hubs;
using StreamWorks.Library.Models.Users.UserData;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StreamWorks.Core.App;

public class AppStateCore : IAppStateCore
{
    private ILogger<AppStateCore> Logger;
    private IUserAppStateData _appStateData;
    private IUserAppState _appState;
    private UserAppStateModel userAppStateDataModel;

    private TwitchSetup _twitchSetup;
    private List<GetUserDataModel>? twitchUserData = new();
    private GetUsersResponse? getUserResponse;

    private AuthenticationStateProvider AuthProvider;
    private IStreamWorksUserData _userData;
    private ClaimsPrincipal? loggedInUserAuthState;

    private StreamWorksUserModel loggedInUser;
    private TwitchConnectionModel twitchConnectionData;

    private bool IsLoggedIn = false;
    private bool IsInfoCorrect = false;
    private bool IsFirstLogin = false;
    private bool IsDataUpdated = false;

    private int tryCount = 3;
    private bool getDataFailed = true;
    private bool accessCodeValid = false;

    private string ClientId = "";
    private string AccessToken = "";
    private string HubConnectionId = "";

    private string UserId = "";
    private string? BroadcasterId = "";

    public AppStateCore(ILogger<AppStateCore> logger,
                    IStreamWorksUserData userData,
                    AuthenticationStateProvider authProvider,
                    TwitchSetup twitchSetup,
                    IUserAppStateData appStateData,
                    IUserAppState appState)
    {
        Logger = logger;
        AuthProvider = authProvider;
        _twitchSetup = twitchSetup;
        _userData = userData;
        _appStateData = appStateData;
        _appState = appState;
    }

    public async Task SetUserData()
    {
        // Get the logged in user and set if they are Logged In
        this.loggedInUser = await CheckLoggedInUser();

        await GetOrCreateStateData();

        // Confirm the user Id is correct
        CheckStartingUserData();

        if (IsFirstLogin == false && IsInfoCorrect == true)
        {
            // Update the user state data
            await UpdateUserDataState();

            IsInfoCorrect = false;
            IsDataUpdated = false;
        }　
        else if (IsFirstLogin == true && IsLoggedIn == true)
        {
            await CreateUserStateData();

            IsFirstLogin = false;
            IsInfoCorrect = false;
            IsDataUpdated = false;
        }

        await CheckUserConnections();

        if (IsLoggedIn == true && this.userAppStateDataModel.TwitchAccountConnected == true)
        {
            // TODO: Check if user has a Twitch connection
            await SetTwitchData();
            Logger.LogInformation($"Twitch Data Set: {this.userAppStateDataModel.TwitchUserData.DisplayName ?? "Twitch Data Not Set"}");

            if (IsDataUpdated == true)
            {
                Logger.LogInformation($"Updating user data...");
                await UpdateUserDataState();
            }
        }
    }

    private async Task GetOrCreateStateData()
    {
        if (loggedInUser is not null && IsLoggedIn == true)
        {
            // User is logged in! Get their state data...
            this.userAppStateDataModel = await GetUserStateData();

            if (this.userAppStateDataModel is not null)
            {
                Logger.LogInformation("User State Data found.");
                IsFirstLogin = false;

                _appState = this.userAppStateDataModel;
            }
            else
            {
                // User is logged in, but no state data found, so create new state data
                Logger.LogInformation("User State Data not found. Creating new.");
                this.userAppStateDataModel = CreateUserState();

                IsFirstLogin = true;
            }
        }
        else
        {
            // User is not logged in, so create new state data
            this.userAppStateDataModel = CreateUserState();

            IsFirstLogin = true;

            if (this.userAppStateDataModel is not null)
            {
                Logger.LogInformation("New User State Data created.");
            }
        }
    }

    private async Task<StreamWorksUserModel> CheckLoggedInUser()
    {
        if (this.loggedInUser is null)
        {
            Console.WriteLine($"No logged in user..");

            if (AuthProvider is not null)
            {
                Console.WriteLine($"AuthProvider ready!");

                var authState = await AuthProvider.GetAuthenticationStateAsync();

                if (authState.User.Identity is not null && authState.User.Identity.IsAuthenticated == true)
                {
                    this.loggedInUserAuthState = authState.User;

                    foreach (var identity in this.loggedInUserAuthState.Identities)
                    {
                        identity.Claims.ToList().ForEach(c => Console.WriteLine($"{c.Type}: {c.Value}"));
                    }

                    this.loggedInUser = await AuthProvider.GetUserFromAuth(_userData);
                    Logger.LogInformation($"Current LoggedInUser: {loggedInUser.UserName}");

                    IsLoggedIn = true;

                    return loggedInUser;
                }
                else
                {
                    Logger.LogError($"Please login..");
                    IsLoggedIn = false;
                    return null;
                }
            }
            else
            {
                Logger.LogError($"AuthProvider is null..");
                IsLoggedIn = false;
                return null;
            }
        }
        else
        {
            Logger.LogInformation($"Current LoggedInUser: {this.loggedInUser.UserName}");
            return this.loggedInUser;
        }
    }

    private async Task CheckUserConnections()
    {
        if (this.loggedInUser is not null)
        {
            if (this.loggedInUser.Logins.Where(l => l.LoginProvider == "TwitchLogin").Count() >= 1)
            {
                this.userAppStateDataModel.TwitchAccountConnected = true;
            }
            else
            {
                this.userAppStateDataModel.TwitchAccountConnected = false;
            }
        }
    }

    private UserAppStateModel CreateUserState()
    {
        if (this.userAppStateDataModel is null)
        {
            this.userAppStateDataModel = new UserAppStateModel();
        }

        return this.userAppStateDataModel;
    }

    private void CheckStartingUserData()
    {
        if (IsLoggedIn == true)
        {
            // User is Logged In and App State exists
            if (this.userAppStateDataModel.UserId != loggedInUser.Id)
            {
                // Currently loaded user state data ID doesnt match logged in user 
                if (this.userAppStateDataModel.UserId == Guid.Empty)
                {
                    // If it is because the Guid is new and currently empty, set it to the current user
                    this.userAppStateDataModel.UserId = loggedInUser.Id;
                    this.userAppStateDataModel.DisplayName = loggedInUser.DisplayName ?? "User";
                    this.userAppStateDataModel.IsLoggedIn = true;
                    this.userAppStateDataModel.LastLogin = DateTimeOffset.Now;

                    IsInfoCorrect = true;
                    IsDataUpdated = true;
                }
                else
                {
                    // TODO Need to handle this?
                    Logger.LogWarning("User State Data User Id is set, but incorrect for this user.");

                    IsInfoCorrect = false;
                    IsDataUpdated = false;
                }
            }
            else
            {
                // Both User Ids match!
                Logger.LogInformation("User State Data User Id is correct for this user.");

                this.userAppStateDataModel.IsLoggedIn = true;
                this.userAppStateDataModel.LastLogin = DateTimeOffset.Now;

                IsInfoCorrect = true;
                IsDataUpdated = true;
            }
        }
        else
        {
            // User is not logged in so set everything to empty
            Logger.LogWarning("User is not logged in. Setting User Id to empty.");
            this.userAppStateDataModel.UserId = Guid.Empty;
            this.userAppStateDataModel.DisplayName = "User";

            IsInfoCorrect = true;
            IsDataUpdated = false;
        }
    }

    private async Task UpdateUserDataState()
    {
        if (IsLoggedIn is true)
        {
            if (IsDataUpdated == true)
            {
                await UpdateUserStateData();
                IsDataUpdated = false;
            }
        }
    }

    private async Task CreateUserStateData()
    {
        try
        {
            await _appStateData.CreateStateData(userAppStateDataModel);
        }
        catch
        {
            Logger.LogError("There was an error creating new user's App State Data.");
        }
    }

    public async Task UpdateUserStateData()
    {
        if (this.userAppStateDataModel.UserId != Guid.Empty)
        {
            try
            {
                await _appStateData.UpdateStateData(this.userAppStateDataModel);
            }
            catch (Exception ex)
            {
                Logger.LogError($"There was an error updating the current user's App State Data. {ex.Message}");
            }
            finally
            {
                Logger.LogInformation("User State Data updated.");
                _appState = this.userAppStateDataModel;
            }
        }
        else
        {
            Logger.LogError("User State Data is null. Cannot update user state data.");
        }
    }

    public async Task<UserAppStateModel> GetUserStateData()
    {
        try
        {
            //var result = await _appStateData.GetAllStateData();
            var result = await _appStateData.GetStateDataByUserId(this.loggedInUser.Id);

            if (result != null)
            {
                //var data = result.Where(r => r.UserId == this.loggedInUser.Id);
                //this.userAppStateDataModel = data.First();
                this.userAppStateDataModel = result.First();
            }
            return this.userAppStateDataModel;
        }
        catch
        {
            Logger.LogError("There was an error getting the current user's App State Data.");
            return null;
        }
    }

    public async Task SetTwitchData()
    {
        this.userAppStateDataModel.TwitchConnection = new TwitchConnectionModel();
        getDataFailed = true;

        if (this.loggedInUser?.Logins.Where(l => l.LoginProvider == "TwitchLogin").Count() >= 1)
        {
            // If the loggedInMember has a Twitch Login, set up the Twitch User
            this.userAppStateDataModel.TwitchConnection = _twitchSetup.SetupTwitchUser(this.loggedInUser);

            // Then try connecting to API to test the Access Token
            while (getDataFailed == true || tryCount > 0)
            {
                Logger.LogWarning($"Current Try: {tryCount}");
                tryCount--;

                // Try to get the User Data from Twitch API
                var result = await _twitchSetup.GetTwitchUserData(this.userAppStateDataModel, this.loggedInUser);

                // If we get the data, break the loop by setting 
                if (result is not null)
                {
                    accessCodeValid = true;
                    this.userAppStateDataModel = result;

                    Logger.LogInformation($"User Data Received: {this.userAppStateDataModel.TwitchUserData.DisplayName}");

                    // No more need to refresh
                    getDataFailed = false;
                }
                // If we don't get data back, then try refreshing the token then trying again!
                else if (result is null)
                {
                    // If we don't get the data and must refresh token, try to refresh the token
                    Logger.LogWarning($"Trying Token Refresh....");
                    accessCodeValid = false;

                    // Try to refresh the Token
                    var newConnectionData = await _twitchSetup.RefreshTwitchToken(this.userAppStateDataModel.TwitchConnection, this.loggedInUser);

                    // If the token is not null, set the new token
                    if (String.IsNullOrEmpty(newConnectionData.AccessToken) is false)
                    {
                        Logger.LogInformation($"Token Refreshed. {loggedInUser?.GetToken("TwitchLogin", "access_token").Value}");
                        this.userAppStateDataModel.TwitchConnection = newConnectionData;

                        AccessToken = this.userAppStateDataModel.TwitchConnection.AccessToken;

                        IsDataUpdated = true;
                    }

                    getDataFailed = true;
                }
                else
                {
                    // Token is invalid
                    accessCodeValid = false;
                    Logger.LogError("There was an error with the API call.");
                    break;
                }
            }

            // If we have the data, set up the Twitch API
            tryCount = 3;
            IsDataUpdated = true;

            if (accessCodeValid == true)
            {
                Logger.LogInformation($"Arrived at Setting up Twitch API...");
                //await TwitchApiSetup();
            }
        }
        else
        {
            Logger.LogError("This User doesn't have a Twitch account connected..");
        }
    }

}
