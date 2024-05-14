using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace StreamWorks.Data.MongoDB;

[CollectionName("Users")]
public class StreamWorksUser : MongoIdentityUser<Guid>
{
}
