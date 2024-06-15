global using System.Security.Claims;
global using System.Net.Http.Headers;
global using System.Timers;

global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.SignalR.Client;
global using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

global using AspNet.Security.OAuth.Twitch;

global using StreamWorks.Components.Account;
global using StreamWorks.Helpers.Users;
global using StreamWorks.Helpers.Twitch;
global using StreamWorks.Library.Models.TwitchApi.Users;
global using StreamWorks.Library.Models.TwitchApi.Config;

global using StreamWorks.Library.DataAccess;
global using StreamWorks.Library.DataAccess.MongoDB.Identity;
global using StreamWorks.Library.Models.Users.Identity;
global using StreamWorks.Library.DataAccess.MongoDB.StreamWorks.AppUserData;

global using TwitchLib.Api;
global using TwitchLib.Api.Helix.Models.EventSub;
