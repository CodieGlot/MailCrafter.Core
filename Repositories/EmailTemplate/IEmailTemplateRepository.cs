using MailCrafter.Domain;

namespace MailCrafter.Repositories;
public interface IEmailTemplateRepository : IMongoCollectionRepostioryBase<EmailTemplateEntity>
{

}