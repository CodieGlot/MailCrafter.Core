using MailCrafter.Domain;
using MailCrafter.Repositories;

namespace MailCrafter.Services;
public class EmailTemplateService : IEmailTemplateService
{
    private readonly IEmailTemplateRepository _emailTemplateRepository;
    public EmailTemplateService(IEmailTemplateRepository emailTemplateRepository)
    {
        _emailTemplateRepository = emailTemplateRepository;
    }
    public async Task<MongoInsertResult> Create(EmailTemplateEntity emailTemplate, bool setExpiration = false)
    {
        emailTemplate.ExpiresAt = setExpiration ? emailTemplate.CreatedAt.AddHours(1) : null;
        return await _emailTemplateRepository.CreateAsync(emailTemplate);
    }
    public async Task<MongoDeleteResult> Delete(string id)
    {
        return await _emailTemplateRepository.DeleteAsync(id);
    }
    public async Task<EmailTemplateEntity?> GetById(string id)
    {
        return await _emailTemplateRepository.GetByIdAsync(id);
    }
    public async Task<MongoReplaceResult> Update(EmailTemplateEntity emailTemplate)
    {
        return await _emailTemplateRepository.ReplaceAsync(emailTemplate.ID, emailTemplate);
    }
}