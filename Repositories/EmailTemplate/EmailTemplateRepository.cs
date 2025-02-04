using MailCrafter.Domain;

namespace MailCrafter.Repositories;
public class EmailTemplateRepository : MongoCollectionRepostioryBase<EmailTemplateEntity>, IEmailTemplateRepository
{
    public EmailTemplateRepository(IMongoDBRepository mongoDBRepository) : base("EmailTemplates", mongoDBRepository)
    {
    }
}