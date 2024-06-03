using Microsoft.AspNetCore.SignalR.Client;
using TwitchLib.Api;
using TwitchLib.EventSub.Websockets;

namespace StreamWorks.Library.Models.Connections.TwitchEvent;
public class EventSubConnectionModel
{
    [BsonId]
    public string? Id { get; set; }
    public Guid? StreamWorksUserId { get; set; }
    public HubConnection? HubConnection { get; set; }
    public EventSubWebsocketClient? EventSubWebsocketClient { get; set; }
    public TwitchAPI? TwitchApiClient { get; set; }
}
