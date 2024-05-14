using Microsoft.AspNetCore.Identity;
using StreamWorks.Data;

namespace StreamWorks.Components.Account;
internal sealed class IdentityUserAccessor(UserManager<StreamWorksUser> userManager, IdentityRedirectManager redirectManager)
{
    public async Task<StreamWorksUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}
