﻿using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using StreamWorks.Helpers.OIDC;
using StreamWorks.Connections;
using TwitchLib.EventSub.Websockets.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using StreamWorks.Library.DataAccess.MongoDB.StreamWorks.Widgets.Timers;
using StreamWorks.Connections.Scopes;

namespace StreamWorks;

public static class RegisterServices
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddMemoryCache();
        builder.Services.AddRazorPages();
        builder.Services.AddResponseCompression(options =>
        {
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" });
        });

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
        builder.Services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, IdentityConstants.ExternalScheme);

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy =>
            {
                //policy.RequireClaim("jobTitle", "Admin");
                policy.RequireRole("Administrator");
            });
        });

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 10;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;

            options.ClaimsIdentity.UserIdClaimType = "user_id";
            options.ClaimsIdentity.UserNameClaimType = "user_name";
            options.ClaimsIdentity.EmailClaimType = "user_email";
        });

        var mongoDbSettings = builder.Configuration.GetSection("MongoDbUserConfig");
        builder.Services.AddIdentity<StreamWorksUserModel, StreamWorksRoleModel>()
        .AddMongoDbStores<StreamWorksUserModel, StreamWorksRoleModel, Guid>
        (
            builder.Configuration.GetConnectionString("MongoDBUsers"),
            mongoDbSettings.GetSection("DatabaseName").Value
        );

        builder.Services.AddIdentityCore<StreamWorksUserModel>(options => options.SignIn.RequireConfirmedAccount = true)
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            .AddRoles<StreamWorksRoleModel>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(options => {
                //options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultScheme = TwitchAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
           .AddTwitch("TwitchLogin", twitchOptions =>
           {
               twitchOptions.ClientId = builder.Configuration["Twitch:ClientId"]!;
               twitchOptions.ClientSecret = builder.Configuration["Twitch:ClientSecret"]!;
               //twitchOptions.CallbackPath = "/signin-twitch";

               twitchOptions.ClaimActions.MapJsonKey("aud", "client_id", "string");
               twitchOptions.ClaimActions.MapJsonKey("preferred_username", "twitch_name", "string");
               twitchOptions.ClaimActions.MapJsonKey("picture", "twitch_picture", "url");

               twitchOptions.Scope.Add("user:read:chat");
               twitchOptions.Scope.Add("moderator:read:followers");
               twitchOptions.Scope.Add("channel:read:subscriptions");
               twitchOptions.Scope.Add("bits:read");
               twitchOptions.Scope.Add("openid");

               // All the scopes to request Twitch for
               //AuthScopes[] scopes = [
               //    AuthScopes.OpenId,
               //    AuthScopes.Helix_Analytics_Read_Extensions,
               //    AuthScopes.Helix_Analytics_Read_Games,
               //    AuthScopes.Helix_Bits_Read,
               //    AuthScopes.Helix_Channel_Edit_Commercial,
               //    AuthScopes.Helix_Channel_Manage_Broadcast,
               //    AuthScopes.Helix_Channel_Manage_Extensions,
               //    AuthScopes.Helix_Channel_Manage_Moderators,
               //    AuthScopes.Helix_Channel_Manage_Polls,
               //    AuthScopes.Helix_Channel_Manage_Predictions,
               //    AuthScopes.Helix_Channel_Manage_Redemptions,
               //    AuthScopes.Helix_Channel_Manage_Schedule,
               //    AuthScopes.Helix_Channel_Manage_VIPs,
               //    AuthScopes.Helix_Channel_Manage_Videos,
               //    AuthScopes.Helix_Channel_Read_Charity,
               //    AuthScopes.Helix_Channel_Read_Editors,
               //    AuthScopes.Helix_Channel_Read_Goals,
               //    AuthScopes.Helix_Channel_Read_Hype_Train,
               //    AuthScopes.Helix_Channel_Read_Polls,
               //    AuthScopes.Helix_Channel_Read_Predictions,
               //    AuthScopes.Helix_Channel_Read_Redemptions,
               //    AuthScopes.Helix_Channel_Read_Stream_Key,
               //    AuthScopes.Helix_Channel_Read_Subscriptions,
               //    AuthScopes.Helix_Channel_Read_VIPs,
               //    AuthScopes.Helix_User_Edit,
               //    AuthScopes.Helix_User_Edit_Broadcast,
               //    AuthScopes.Helix_User_Edit_Follows,mn
               //    AuthScopes.Helix_User_Manage_BlockedUsers,
               //    AuthScopes.Helix_User_Manage_Chat_Color, 
               //    AuthScopes.Helix_User_Manage_Whispers,
               //    AuthScopes.Helix_User_Read_BlockedUsers,
               //    AuthScopes.Helix_User_Read_Broadcast,
               //    AuthScopes.Helix_User_Read_Email,
               //    AuthScopes.Helix_User_Read_Follows,
               //    AuthScopes.Helix_User_Read_Subscriptions,
               //    AuthScopes.Helix_moderator_Manage_Chat_Messages,
               //];

               //scopes.Select(AuthScopesToString)
               //      .ToList()
               //      .ForEach(twitchOptions.Scope.Add);

               twitchOptions.SaveTokens = true;
           })
           .AddBearerToken(TwitchAuthenticationDefaults.AuthenticationScheme);
           //.AddIdentityCookies();

        builder.Services.AddSingleton<IEmailSender<StreamWorksUserModel>, IdentityNoOpEmailSender>();

        //builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();
        builder.Services.AddHttpClient("twitchApi", config => { 
            config.BaseAddress = new Uri("https://api.twitch.tv/helix/");
            config.DefaultRequestHeaders.Add("Client-ID", builder.Configuration["Twitch:ClientId"]!);
        });

        builder.Services.AddBlazorBootstrap();

        builder.Services.AddSingleton<TwitchAPI>();
        //builder.Services.AddSingleton<TwitchUserController>();
        //builder.Services.AddSingleton<TwitchEventSubController>();

        // Common Data Services
        builder.Services.AddSingleton<IDbUserConnection, DbUserConnection>();
        builder.Services.AddSingleton<IStreamWorksUserData, MongoStreamWorksUserData>();
        builder.Services.AddSingleton<IDbStreamWorksConnection, DbStreamWorksConnection>();

        // Twitch Data Services
        builder.Services.AddTwitchLibEventSubWebsockets();
        //builder.Services.AddHostedService<TwitchEventSubConnection>();
        builder.Services.AddHostedService<TwitchEventSubConnectionService>();
        builder.Services.AddScoped<IScopedEventSubConnection, ScopedEventSubConnectionTasks>();

        builder.Services.AddScoped<ITwitchSignInHelpers, TwitchSignInHelpers>();

        // Widgets
        builder.Services.AddScoped<IStreamWorksTimerData, MongoStreamWorksTimerData>();
    }
}
