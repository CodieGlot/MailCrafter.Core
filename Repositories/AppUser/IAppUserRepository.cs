using MailCrafter.Domain;

namespace MailCrafter.Repositories;

public interface IAppUserRepository : IMongoCollectionRepostioryBase<AppUserEntity>
{
}
