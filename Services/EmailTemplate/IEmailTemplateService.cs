using MailCrafter.Domain;

namespace MailCrafter.Services;
public interface IEmailTemplateService : IBasicOperations<EmailTemplateEntity>
{
    Task<List<EmailTemplateEntity>> GetPageQueryDataAsync(PageQueryDTO queryDTO);
}