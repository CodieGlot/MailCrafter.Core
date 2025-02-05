using MailCrafter.Domain;

namespace MailCrafter.Services;

public interface IEmailService
{
    Task<OperationResult> SendAsync(string templateId, EmailDetailsDto details, bool isUsingSystemTemplate = false);
}
