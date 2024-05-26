
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Mvc;
using StreamWorks.Api.Twitch.Models.User;
using StreamWorks.ApiLibrary.Twitch.Models.Config;
using StreamWorks.ApiLibrary.Twitch.Models.Users;
using System.Text.Json;
using System.Text.Json.Serialization;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Api.Interfaces;

namespace StreamWorks.Api.Twitch.Controllers.User;
[Route("api/[controller]")]
[ApiController]
public class TwitchUserController : ControllerBase
{
    private readonly ILogger<TwitchUserController> _logger;
    private readonly IConfiguration _config;
    private HubConnection? hubConnection;
    private readonly TwitchAPI _twitchApi;

    public TwitchUserController(ILogger<TwitchUserController> logger, IConfiguration config, TwitchAPI api)
    {
        _logger = logger;
        _config = config;
        _twitchApi = api;

        _twitchApi.Settings.ClientId = _config["Twitch:ClientId"];
        _twitchApi.Settings.Secret = _config["Twitch:ClientSecret"];
    }

    [HttpGet(Name = "GetTwitchUsers")]
    public async Task<GetUsersResponse> GetTwitchUserData(string id, string accessCode)
    {
        _twitchApi.Settings.AccessToken = accessCode;

        try
        {
            var user = await _twitchApi.Helix.Users.GetUsersAsync(logins: new List<string> { id });

            //var userdata = new GetUserDataModel
            //{
            //    BroadcasterType = user.Users[0].BroadcasterType,
            //    CreatedAt = user.Users[0].CreatedAt,
            //    Description = user.Users[0].Description,
            //    DisplayName = user.Users[0].DisplayName,
            //    Id = user.Users[0].Id,
            //    Login = user.Users[0].Login,
            //    OfflineImageUrl = user.Users[0].OfflineImageUrl,
            //    ProfileImageUrl = user.Users[0].ProfileImageUrl,
            //    Type = user.Users[0].Type,
            //    ViewCount = user.Users[0].ViewCount
            //};
 
            //ActionResult result = Ok(user);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting user data from Twitch. Error Code: {ex.HResult}");
            //return StatusCode(ex.HResult);
            return null;
        }
    }
}
