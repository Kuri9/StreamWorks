using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace StreamWorks.Data.MongoDB;

[CollectionName("Roles")]
public class StreamWorksRole : MongoIdentityRole<Guid>
{
}
