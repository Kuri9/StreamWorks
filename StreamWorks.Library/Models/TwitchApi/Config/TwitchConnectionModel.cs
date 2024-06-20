using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamWorks.Library.Models.TwitchApi.Config;
public class TwitchConnectionModel
{
    public string? TwitchId { get; set; }
    public string? BroadcasterId { get; set; }
    public string? ModeratorId { get; set; }
    public string? ClientId { get; set; }
    public string? AccessToken { get; set; }
    public string? ClientSecret { get; set; }
    public string? RedirectUri { get; set; }
    public TwitchScope Scopes { get; set; } = new TwitchScope();
    public string? RefreshToken { get; set; }
    public string? Code { get; set; }
}
