using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace StreamWorks;

public static class RegisterServices
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

        //builder.Services.AddAuthentication(options =>
        //{
        //    options.DefaultScheme = IdentityConstants.ApplicationScheme;
        //    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        //})
        //    .AddIdentityCookies();

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

        builder.Services.AddSingleton<IEmailSender<StreamWorksUser>, IdentityNoOpEmailSender>();
    }
}
