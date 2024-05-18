﻿using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using StreamWorks.Helpers.OIDC;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets.Extensions;
using static TwitchLib.Api.Core.Common.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using AspNet.Security.OAuth.Twitch;
using static System.Net.WebRequestMethods;

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

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
        builder.Services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, IdentityConstants.ExternalScheme);

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 10;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
        });

        var mongoDbSettings = builder.Configuration.GetSection("MongoDbConfig");
        builder.Services.AddIdentity<StreamWorksUser, StreamWorksRole>()
        .AddMongoDbStores<StreamWorksUser, StreamWorksRole, Guid>
        (
            mongoDbSettings.GetConnectionString("MongoDB"),
            mongoDbSettings.GetSection("DatabaseName").Value
        );

        builder.Services.AddIdentityCore<StreamWorksUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            .AddRoles<StreamWorksRole>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(options => {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                //options.DefaultScheme = TwitchAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
           .AddTwitch("TwitchLogin", twitchOptions =>
           {
               twitchOptions.ClientId = builder.Configuration["Twitch:ClientId"]!;
               twitchOptions.ClientSecret = builder.Configuration["Twitch:ClientSecret"]!;

               // All the scopes to request Twitch for
               AuthScopes[] scopes = [
                   AuthScopes.OpenId,
                   AuthScopes.Helix_Analytics_Read_Extensions,
                   AuthScopes.Helix_Analytics_Read_Games,
                   AuthScopes.Helix_Bits_Read,
                   AuthScopes.Helix_Channel_Edit_Commercial,
                   AuthScopes.Helix_Channel_Manage_Broadcast,
                   AuthScopes.Helix_Channel_Manage_Extensions,
                   AuthScopes.Helix_Channel_Manage_Moderators,
                   AuthScopes.Helix_Channel_Manage_Polls,
                   AuthScopes.Helix_Channel_Manage_Predictions,
                   AuthScopes.Helix_Channel_Manage_Redemptions,
                   AuthScopes.Helix_Channel_Manage_Schedule,
                   AuthScopes.Helix_Channel_Manage_VIPs,
                   AuthScopes.Helix_Channel_Manage_Videos,
                   AuthScopes.Helix_Channel_Read_Charity,
                   AuthScopes.Helix_Channel_Read_Editors,
                   AuthScopes.Helix_Channel_Read_Goals,
                   AuthScopes.Helix_Channel_Read_Hype_Train,
                   AuthScopes.Helix_Channel_Read_Polls,
                   AuthScopes.Helix_Channel_Read_Predictions,
                   AuthScopes.Helix_Channel_Read_Redemptions,
                   AuthScopes.Helix_Channel_Read_Stream_Key,
                   AuthScopes.Helix_Channel_Read_Subscriptions,
                   AuthScopes.Helix_Channel_Read_VIPs,
                   AuthScopes.Helix_User_Edit,
                   AuthScopes.Helix_User_Edit_Broadcast,
                   AuthScopes.Helix_User_Edit_Follows,
                   AuthScopes.Helix_User_Manage_BlockedUsers,
                   AuthScopes.Helix_User_Manage_Chat_Color,
                   AuthScopes.Helix_User_Manage_Whispers,
                   AuthScopes.Helix_User_Read_BlockedUsers,
                   AuthScopes.Helix_User_Read_Broadcast,
                   AuthScopes.Helix_User_Read_Email,
                   AuthScopes.Helix_User_Read_Follows,
                   AuthScopes.Helix_User_Read_Subscriptions,
                   AuthScopes.Helix_moderator_Manage_Chat_Messages,
               ];
               scopes
                   .Select(AuthScopesToString)
                   .ToList()
               .ForEach(twitchOptions.Scope.Add);

               //twitchOptions.AuthorizationEndpoint = "/Account/Manage/ExternalLogins";
               twitchOptions.SaveTokens = true;
           })
           .AddBearerToken(TwitchAuthenticationDefaults.AuthenticationScheme);
           //.AddIdentityCookies();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy =>
            {
                //policy.RequireClaim("jobTitle", "Admin");
                policy.RequireRole("Administrator");
            });
        });

        builder.Services.AddSingleton<IEmailSender<StreamWorksUser>, IdentityNoOpEmailSender>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddBlazorBootstrap();
    }
}
