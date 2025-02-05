using MailCrafter.Domain;

namespace MailCrafter.Services;
public interface IEmailSendingService
{
    Task SendBasicEmailsAsync(BasicEmailDetailsModel details);
    Task SendPersonalizedEmailsAsync(PersonalizedEmailDetailsModel details);
}