using MailCrafter.Domain;
using MailCrafter.Utils.Helpers;

namespace MailCrafter.Services;

public class EmailService : IEmailService
{
    private readonly IAesEncryptionHelper _encryptionHelper;
    public Task<OperationResult> SendAsync(string templateId, EmailDetailsDto details, bool isUsingSystemTemplate = false)
    {
        throw new NotImplementedException();
    }
}
