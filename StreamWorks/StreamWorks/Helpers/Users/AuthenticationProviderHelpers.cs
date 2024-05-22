using Microsoft.AspNetCore.Components.Authorization;
using MongoDB.Bson;
using StreamWorks.Library.DataAccess.MongoDB.Identity;
using StreamWorks.Library.Models.Users.Identity;
using static AspNet.Security.OAuth.Twitch.TwitchAuthenticationConstants;

namespace StreamWorks.Helpers.Users;

public static class AuthenticationProviderHelpers
{
    /// <summary>
    /// Extends the AuthenticationStateProvider by adding a way to look up a user based on their
    /// objectidentifier from the Azure User database based on the loggedin users details.
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="userData"></param>
    /// <returns>objectId - User Id from Azure AD B2C</returns>

    public static async Task<StreamWorksUserModel> GetUserFromAuth(
        this AuthenticationStateProvider provider,
        IStreamWorksUserData userData)
    {
        var authState = await provider.GetAuthenticationStateAsync();
        string objectId = authState?.User.Claims.FirstOrDefault(c => c.Type.Contains("user_id"))?.Value;
        // var result = await userData.GetUserFromAuthentication(objectId);
        var result = await userData.GetUser(objectId);
        return result;
    }

    /// <summary>
    /// Checks if the loogedInUsers data is the same as what we have in the database for our User.
    /// If no user with the same ID exists in our database, it makes one. Otherwise, if the information
    /// is different, it updates our database based on the changes the user made in Azure.
    /// </summary>
    /// <returns></returns>

    //public static async Task<StreamWorksUserModel> LoadAndVerifyUser(
    //    this AuthenticationStateProvider provider,
    //    IUserData userData
    //    )
    //{
    //    //TODO: Probably need to move this into the Helper so it can be used on other pages as well.
    //    var loggedInUser = await provider.GetUserFromAuth(userData);

    //    var authState = await provider.GetAuthenticationStateAsync();
    //    string objectId = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier"))?.Value;

    //    if (string.IsNullOrWhiteSpace(objectId) == false)
    //    {
    //        loggedInUser = await userData.GetUserFromAuthentication(objectId) ?? new();

    //        string firstName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("givenname"))?.Value;
    //        string lastName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("surname"))?.Value;
    //        string displayName = authState.User.Claims.FirstOrDefault(c => c.Type.Equals("name"))?.Value;
    //        string email = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value;

    //        bool isDirty = false;

    //        if (objectId.Equals(loggedInUser.ObjectIdentifier) == false)
    //        {
    //            isDirty = true;
    //            loggedInUser.ObjectIdentifier = objectId;
    //        }
    //        if (firstName.Equals(loggedInUser.FirstName) == false)
    //        {
    //            isDirty = true;
    //            loggedInUser.FirstName = firstName;
    //        }
    //        if (lastName.Equals(loggedInUser.LastName) == false)
    //        {
    //            isDirty = true;
    //            loggedInUser.LastName = lastName;
    //        }
    //        if (displayName.Equals(loggedInUser.DisplayName) == false)
    //        {
    //            isDirty = true;
    //            loggedInUser.DisplayName = displayName;
    //        }
    //        if (email.Equals(loggedInUser.Email) == false)
    //        {
    //            isDirty = true;
    //            loggedInUser.Email = email;
    //        }

    //        if (isDirty)
    //        {
    //            if (string.IsNullOrWhiteSpace(loggedInUser.Id))
    //            {
    //                await userData.CreateUser(loggedInUser);
    //            }
    //            else
    //            {
    //                await userData.UpdateUser(loggedInUser);
    //            }
    //        }
    //    }

    //    return loggedInUser;
    //}
}
