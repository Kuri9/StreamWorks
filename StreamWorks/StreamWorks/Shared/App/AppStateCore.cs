using Microsoft.AspNetCore.Components.Authorization;
using StreamWorks.Library.Models.Users.UserData;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StreamWorks.Shared.App;

public class AppStateCore
{
    public StreamWorksUserModel? loggedInUser { get; private set; }
    public UserAppStateModel userAppStateDataModel { get; private set; }
    public TwitchConnectionModel? twitchConnectionData { get; private set; }

    public string? UserId { get; private set; } = "";
    public string? BroadcasterId { get; private set; } = "";
    public string? ClientId { get; private set; } = "";
    public string? AccessToken { get; private set; } = "";
    public string? HubConnectionId { get; private set; } = "";

    // Private variables
    private ILogger<AppStateCore> Logger;
    private IUserAppStateData _appStateData;
    private IStreamEventLogData _eventLogData;
    private IUserAppState _appState;

    private TwitchSetup _twitchSetup;
    private List<GetUserDataModel>? twitchUserData = new();
    private GetUsersResponse? getUserResponse;

    private AuthenticationStateProvider AuthProvider;
    private IStreamWorksUserData _userData;
    private ClaimsPrincipal? loggedInUserAuthState;

    private bool IsLoggedIn = false;
    private bool IsInfoCorrect = false;
    private bool IsFirstLogin = false;
    private bool IsDataUpdated = false;

    private int tryCount = 3;
    private bool accessCodeValid = false;

    public AppStateCore(ILogger<AppStateCore> logger,
                    IStreamWorksUserData userData,
                    AuthenticationStateProvider authProvider,
                    TwitchSetup twitchSetup,
                    IUserAppStateData appStateData,
                    IStreamEventLogData eventLogData,
                    IUserAppState appState)
    {
        Logger = logger;
        AuthProvider = authProvider;
        _twitchSetup = twitchSetup;
        _userData = userData;
        _appStateData = appStateData;
        _eventLogData = eventLogData;
        _appState = appState;

    }

    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();


    // BASIC STATE METHODS
    public async Task SetUser(StreamWorksUserModel user)
    {
        loggedInUser = user;
        await UpdateUserStateData();
        NotifyStateChanged();
    }

    public async Task SetUserState(UserAppStateModel userState)
    {
        userAppStateDataModel = userState;
        await UpdateUserStateData();
        NotifyStateChanged();
    }

    public async Task SetTwitchConnection(TwitchConnectionModel twitchConnection)
    {
        twitchConnectionData = twitchConnection;
        await UpdateUserStateData();
        NotifyStateChanged();
    }

    public async Task SetDisplayName(string displayName)
    {
        userAppStateDataModel.DisplayName = displayName;
        await UpdateUserStateData();
        NotifyStateChanged();
    }

    public async Task SetStreamingStatus(bool streamingStatus)
    {
        userAppStateDataModel.IsStreaming = streamingStatus;
        await UpdateUserStateData();
        NotifyStateChanged();
    }

    public async Task LoginUser()
    {
        userAppStateDataModel.LastLogin = DateTimeOffset.Now;
        SetLoggedInStatus(true);

        await UpdateUserStateData();
        NotifyStateChanged();
    }

    public async Task LogoutUser()
    {
        userAppStateDataModel.LastLogout = DateTimeOffset.Now;
        SetLoggedInStatus(false);

        await UpdateUserStateData();
        NotifyStateChanged();
    }

    private void SetLoggedInStatus(bool loggedInStatus)
    {
        userAppStateDataModel.IsLoggedIn = loggedInStatus;
        IsLoggedIn = loggedInStatus;
    }

    // INITIALIZATION METHODS
    public async Task Initialize()
    {
        // Get the logged in user and set if they are Logged In
        loggedInUser = await CheckLoggedInUser();

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

        CheckUserConnections();

        if (IsLoggedIn == true && userAppStateDataModel?.TwitchAccountConnected == true)
        {
            // TODO: Check if user has a Twitch connection
            await SetTwitchData();
            Logger.LogInformation($"Twitch Data Set: {userAppStateDataModel.TwitchUserData.DisplayName ?? "Twitch Data Not Set"}");

            var eventLog = await _eventLogData.GetOrCreateLogData(loggedInUser.Id);
            if (eventLog is not null)
            {
                Logger.LogInformation($"Event Log Data: {eventLog.UserId}");
                userAppStateDataModel.EventLogs = eventLog;
            }

            if (IsDataUpdated == true)
            {
                Logger.LogInformation($"Updating user data...");
                await UpdateUserDataState();
            }

            if (accessCodeValid == true)
            {
                await _twitchSetup.TwitchApiSetup(userAppStateDataModel);
            }
        }

        NotifyStateChanged();
    }

    private async Task GetOrCreateStateData()
    {
        if (loggedInUser is not null && IsLoggedIn == true)
        {
            // User is logged in! Get their state data...
            userAppStateDataModel = await GetUserStateData();

            if (userAppStateDataModel is not null)
            {
                Logger.LogInformation("User State Data found.");
                IsFirstLogin = false;

                _appState = userAppStateDataModel;
            }
            else
            {
                // User is logged in, but no state data found, so create new state data
                Logger.LogInformation("User State Data not found. Creating new.");
                userAppStateDataModel = CreateUserState();

                IsFirstLogin = true;
            }
        }
        else
        {
            // User is not logged in, so create new state data
            userAppStateDataModel = CreateUserState();

            IsFirstLogin = true;

            if (userAppStateDataModel is not null)
            {
                Logger.LogInformation("New User State Data created.");
            }
        }
    }

    private async Task<StreamWorksUserModel> CheckLoggedInUser()
    {
        if (loggedInUser is null)
        {
            Console.WriteLine($"No logged in user..");

            if (AuthProvider is not null)
            {
                Console.WriteLine($"AuthProvider ready!");
                var authState = await AuthProvider.GetAuthenticationStateAsync();

                if (authState.User.Identity is not null && authState.User.Identity.IsAuthenticated == true)
                {
                    loggedInUserAuthState = authState.User;

                    //foreach (var identity in loggedInUserAuthState.Identities)
                    //{
                    //    identity.Claims.ToList().ForEach(c => Console.WriteLine($"{c.Type}: {c.Value}"));
                    //}

                    loggedInUser = await AuthProvider.GetUserFromAuth(_userData);
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
            Logger.LogInformation($"Current LoggedInUser: {loggedInUser.UserName}");
            return loggedInUser;
        }
    }

    // Check what external accounts the user has connected
    private void CheckUserConnections()
    {
        if (loggedInUser is not null)
        {
            if (loggedInUser.Logins.Where(l => l.LoginProvider == "TwitchLogin").Count() >= 1)
            {
                userAppStateDataModel.TwitchAccountConnected = true;
            }
            else
            {
                userAppStateDataModel.TwitchAccountConnected = false;
            }
        }
    }

    private UserAppStateModel CreateUserState()
    {
        if (userAppStateDataModel is null)
        {
            userAppStateDataModel = new UserAppStateModel();
        }

        return userAppStateDataModel;
    }

    private void CheckStartingUserData()
    {
        if (IsLoggedIn == true)
        {
            // User is Logged In and App State exists
            if (userAppStateDataModel.UserId != loggedInUser.Id)
            {
                // Currently loaded user state data ID doesnt match logged in user 
                if (userAppStateDataModel.UserId == Guid.Empty)
                {
                    // If it is because the Guid is new and currently empty, set it to the current user
                    userAppStateDataModel.UserId = loggedInUser.Id;
                    userAppStateDataModel.DisplayName = loggedInUser.DisplayName ?? "User";
                    userAppStateDataModel.IsLoggedIn = true;
                    userAppStateDataModel.LastLogin = DateTimeOffset.Now;

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

                userAppStateDataModel.IsLoggedIn = true;
                userAppStateDataModel.LastLogin = DateTimeOffset.Now;

                IsInfoCorrect = true;
                IsDataUpdated = true;
            }
        }
        else
        {
            // User is not logged in so set everything to empty
            Logger.LogWarning("User is not logged in. Setting User Id to empty.");
            userAppStateDataModel.UserId = Guid.Empty;
            userAppStateDataModel.DisplayName = "User";

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

    private async Task UpdateUserStateData()
    {
        if (userAppStateDataModel is not null && userAppStateDataModel.UserId != Guid.Empty)
        {
            try
            {
                await _appStateData.UpdateStateData(userAppStateDataModel);
            }
            catch (Exception ex)
            {
                Logger.LogError($"There was an error updating the current user's App State Data. {ex.Message}");
            }
            finally
            {
                Logger.LogInformation("User State Data updated.");
                _appState = userAppStateDataModel;
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
            var result = await _appStateData.GetStateDataByUserId(loggedInUser.Id);

            if (result != null)
            {
                userAppStateDataModel = result.First();
                return userAppStateDataModel;
            }
        }
        catch
        {
            Logger.LogError("There was an error getting the current user's App State Data.");
        }

        return null;
    }

    public async Task SetTwitchData()
    {
        if (userAppStateDataModel is not null)
        {
            accessCodeValid = false;

            if (userAppStateDataModel.TwitchAccountConnected == true)
            {
                // If the loggedInMember has a Twitch Login, set up the Twitch User
                userAppStateDataModel.TwitchConnection = new TwitchConnectionModel();
                SetTwitchConnectionData();

                // Then try connecting to API to test the Access Token
                while (tryCount > 0)
                {
                    if (tryCount < 3)
                    {
                        // If less than two, then Twitch info was updated, so re-update the user info
                        loggedInUser = await CheckLoggedInUser();

                        // If the loggedInMember has a Twitch Login, set up the Twitch User
                        SetTwitchConnectionData();

                        Logger.LogWarning($"Refreshed User Data: {loggedInUser.Id}");
                    }

                    Logger.LogWarning($"Current Try: {tryCount}");
                    tryCount--;

                    UserAppStateModel? result = null;

                    // Try to get the User Data from Twitch API
                    if (loggedInUser is not null)
                    {
                        result = await _twitchSetup.GetTwitchUserData(userAppStateDataModel, loggedInUser);
                    }

                    // If we get the data, break the loop by setting 
                    if (result is not null)
                    {
                        accessCodeValid = true;
                        userAppStateDataModel = result;

                        Logger.LogInformation($"User Data Received: {userAppStateDataModel.TwitchUserData.DisplayName}");
                        break;
                    }
                    // If we don't get data back, then try refreshing the token then trying again!
                    else if (result is null && loggedInUser is not null)
                    {
                        // If we don't get the data and must refresh token, try to refresh the token
                        Logger.LogWarning($"Trying Token Refresh....");

                        // Try to refresh the Token
                        var newConnectionData = await _twitchSetup.RefreshTwitchToken(userAppStateDataModel.TwitchConnection, loggedInUser);

                        // If the token is not null, set the new token
                        if (string.IsNullOrEmpty(newConnectionData.AccessToken) is false)
                        {
                            Logger.LogInformation($"Token Refreshed. {loggedInUser?.GetToken("TwitchLogin", "access_token").Value}");
                            userAppStateDataModel.TwitchConnection = newConnectionData;

                            AccessToken = userAppStateDataModel.TwitchConnection.AccessToken;
                            IsDataUpdated = true;
                        }
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
                }
            }
            else
            {
                Logger.LogError("This User doesn't have a Twitch account connected..");
            }
        }
        else
        {
            Logger.LogError("User State Data is null. Cannot set Twitch Data.");
        }
    }

    private void SetTwitchConnectionData()
    {
        if (userAppStateDataModel is not null && loggedInUser is not null)
        {
            // If the loggedInMember has a Twitch Login, set up the Twitch User
            userAppStateDataModel.TwitchConnection = _twitchSetup.SetupTwitchUser(loggedInUser);
        }
    }
}
