using MailCrafter.Domain;

namespace MailCrafter.Repositories;
public interface IEmailScheduleRepository : IMongoCollectionRepostioryBase<EmailScheduleEntity>
{
}
