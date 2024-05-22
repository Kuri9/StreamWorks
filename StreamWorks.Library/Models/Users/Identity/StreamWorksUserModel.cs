using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using StreamWorks.Library.Models.Users.Twitch;

namespace StreamWorks.Library.Models.Users.Identity;

[CollectionName("Users")]
public class StreamWorksUserModel : MongoIdentityUser<Guid>
{
    public string? DisplayName { get; set; }
    public TwitchAccessDataModel? TwitchAccessData { get; set; }
}
