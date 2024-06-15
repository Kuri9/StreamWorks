using Microsoft.AspNetCore.Components.Authorization;
using StreamWorks.Library.Models.Users.UserData;

namespace StreamWorks.Core.App;

public class AppState
{
    private ILogger<AppState> Logger;
    private IAppStateUserData _appStateData;
    private AuthenticationStateProvider AuthProvider;
    private IStreamWorksUserData _userData;
    private AuthenticationState _authState;
    
    private StreamWorksUserModel loggedInUser;
    private UserAppStateDataModel userAppStateDataModel;
    
    public AppState(ILogger<AppState> logger,
                    IStreamWorksUserData userData,
                    AuthenticationStateProvider authProvider, 
                    AuthenticationState authState, 
                    IAppStateUserData appStateData)
    {
        Logger = logger;
        _userData = userData;
        AuthProvider = authProvider;
        _authState = authState;
        _appStateData = appStateData;

        loggedInUser = new StreamWorksUserModel();
        userAppStateDataModel = new UserAppStateDataModel();
    }

    public async Task<UserAppStateDataModel> SetUserStateData()
    {
        if (_authState.User.Identity is not null && _authState.User.Identity.IsAuthenticated)
        {
            try
            {
                if (_userData == null)
                {
                    Logger.LogError("User data is null. Cannot set user data. Create a new user.");
                    loggedInUser = new StreamWorksUserModel();
                }
                else
                {
                    loggedInUser = await AuthProvider.GetUserFromAuth(_userData);
                }
            }
            catch
            {
                Logger.LogError("Error getting user data. Couldn't connect to database..");
            }

            if (loggedInUser is not null)
            {
                userAppStateDataModel.UserId = loggedInUser.Id;
                userAppStateDataModel.DisplayName = loggedInUser.DisplayName ?? "User";
            }

            return userAppStateDataModel;
        }
        else
        {
            Logger.LogError("User is not authenticated. Cannot set user state data.");
            return null;
        }
    }

    public async Task<UserAppStateDataModel> GetStateData()
    {
        try
        {
            var result = await _appStateData.GetStateDataByUserId(userAppStateDataModel.UserId);
            return userAppStateDataModel;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting user state data");
            return null;
        }
    }

    public async Task<bool> CreateOrUpdateStateData()
    {
        try
        {
            await _appStateData.UpdateStateData(userAppStateDataModel);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating user state data");
            return false;
        }
    }
}
