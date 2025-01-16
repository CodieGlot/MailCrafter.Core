using MailCrafter.Domain;

namespace MailCrafter.Repositories;
public class AppUserRepository : MongoCollectionRepostioryBase<AppUserEntity>, IAppUserRepository
{
    public AppUserRepository(IMongoDBRepository mongoDBRepository) : base("AppUsers", mongoDBRepository)
    {
    }
}
