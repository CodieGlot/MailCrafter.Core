using MailCrafter.Domain;

namespace MailCrafter.Repositories;
public class EmailScheduleRepository : MongoCollectionRepostioryBase<EmailScheduleEntity>, IEmailScheduleRepository
{
    public EmailScheduleRepository(IMongoDBRepository mongoDBRepository) : base("EmailSchedules", mongoDBRepository)
    {
    }
}
