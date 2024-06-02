using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamWorks.Library.Models.TwitchApi.Config;
public class TwitchConnectionModel
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? RedirectUri { get; set; }
    public string[]? Scopes { get; set; }
    public string? RefreshToken { get; set; }
    public string? Code { get; set; }
}
