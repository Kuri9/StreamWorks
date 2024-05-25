using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    private readonly TwitchAPI _twitchApi;

    public TwitchUserController(ILogger<TwitchUserController> logger, TwitchAPI api)
    {
        _logger = logger;
        _twitchApi = api;
    }

    [HttpGet(Name = "GetTwitchUsers")]
    public async Task<ActionResult<GetUserDataModel>> GetTwitchUserData(string id, string accessCode, string clientId)
    {
        _twitchApi.Settings.AccessToken = accessCode;
        _twitchApi.Settings.ClientId = clientId;

        try
        {
            var user = await _twitchApi.Helix.Users.GetUsersAsync(logins: new List<string> { id });

            if (user.Users.Length == 0)
            {
                return BadRequest();
            }

            var userdata = new GetUserDataModel
            {
                BroadcasterType = user.Users[0].BroadcasterType,
                CreatedAt = user.Users[0].CreatedAt,
                Description = user.Users[0].Description,
                DisplayName = user.Users[0].DisplayName,
                Id = user.Users[0].Id,
                Login = user.Users[0].Login,
                OfflineImageUrl = user.Users[0].OfflineImageUrl,
                ProfileImageUrl = user.Users[0].ProfileImageUrl,
                Type = user.Users[0].Type,
                ViewCount = user.Users[0].ViewCount
            };

            ActionResult result = Ok(userdata);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user data from Twitch");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }


}
