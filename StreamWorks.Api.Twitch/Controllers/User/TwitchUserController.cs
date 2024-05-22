using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TwitchLib.Api;
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

    [HttpGet("{id}", Name = "GetTwitchUsers")]
    public async Task<IActionResult> GetTwitchUserData(string id, string accessCode, string clientId)
    {
        _twitchApi.Settings.ClientId = clientId;
        _twitchApi.Settings.AccessToken = accessCode;

        var user = await _twitchApi.Helix.Users.GetUsersAsync(logins: new List<string> { id });
        if (user.Users.Length == 0)
        {
            return NotFound();
        }

        return Ok(user.Users[0]);
    }
}
