using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace StreamWorks.Library.Models.Users.Identity;

[CollectionName("Users")]
public class StreamWorksUserModel : MongoIdentityUser<Guid>
{
}
