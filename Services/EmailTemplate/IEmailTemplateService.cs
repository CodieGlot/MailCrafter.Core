using MailCrafter.Domain;

namespace MailCrafter.Services;
public interface IEmailTemplateService
{
    Task<MongoInsertResult> Create(EmailTemplateEntity emailTemplate, bool setExpiration = false);
    Task<MongoDeleteResult> Delete(string id);
    Task<EmailTemplateEntity?> GetById(string id);
    Task<MongoReplaceResult> Update(EmailTemplateEntity emailTemplate);
}