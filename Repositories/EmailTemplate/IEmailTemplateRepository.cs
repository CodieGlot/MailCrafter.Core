using MailCrafter.Repositories;

namespace MailCrafter.Domain;
public interface IEmailTemplateRepository : IMongoCollectionRepostioryBase<EmailTemplateEntity>
{

}