using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace StreamWorks.Library.Models.Users.Identity;

[CollectionName("Roles")]
public class StreamWorksRoleModel : MongoIdentityRole<Guid>
{
}
