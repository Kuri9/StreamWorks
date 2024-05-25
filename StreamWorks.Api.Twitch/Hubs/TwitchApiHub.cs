using StreamWorks.Api.Twitch.Models.User;

namespace StreamWorks.Api.Twitch.Hubs;

public class TwitchApiHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    // Twitch API Connection Calls
    public async Task<GetUserDataModel> GetUsersDataModel()
    {
        return new GetUserDataModel();
    }
}
