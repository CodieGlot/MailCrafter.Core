using MailCrafter.Domain;

namespace MailCrafter.Repositories;
public class CustomGroupRepository : MongoCollectionRepostioryBase<CustomGroupEntity>, ICustomGroupRepository
{
    public CustomGroupRepository(IMongoDBRepository mongoDBRepository) : base("CustomGroups", mongoDBRepository)
    {
    }
}
