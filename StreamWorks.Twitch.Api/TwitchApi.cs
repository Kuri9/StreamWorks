using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;

namespace StreamWorks.Twitch.Api;

public class TwitchApi
{
    TwitchAPI? api;

    public TwitchApi(TwitchLoginData twitchLoginData)
    {
        api = new TwitchAPI();
        api.Settings.ClientId = twitchLoginData.ClientId;
        api.Settings.AccessToken = twitchLoginData.AccessToken;
    }

    private async Task ExampleCallsAsync()
    {
        //Gets a list of all the subscritions of the specified channel.
        //var allSubscriptions = await api.Helix.Subscriptions.GetBroadcasterSubscriptionsAsync("broadcasterID", null, 100, "accesstoken");

        //Get channels a specified user follows.
        var userFollows = await api.Helix.Users.GetUsersFollowsAsync("user_id");

        //Get Specified Channel Follows
        var channelFollowers = await api.Helix.Users.GetUsersFollowsAsync(fromId: "channel_id");

        //Returns a stream object if online, if channel is offline it will be null/empty.
        //var streams = await api.Helix.Streams.GetStreamsAsync(userIds: userIdsList); // Alternative: userLogins: userLoginsList

        //Update Channel Title/Game/Language/Delay - Only require 1 option here.
        //var request = new ModifyChannelInformationRequest() { GameId = "New_Game_Id", Title = "New stream title", BroadcasterLanguage = "New_Language", Delay = New_Delay };
        //await api.Helix.Channels.ModifyChannelInformationAsync("broadcaster_Id", request, "AccessToken");
    }
}
