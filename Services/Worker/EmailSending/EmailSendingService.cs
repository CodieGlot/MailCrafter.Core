using MailCrafter.Domain;
using MailCrafter.Utils.Helpers;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace MailCrafter.Services;
public class EmailSendingService : IEmailSendingService
{
    private readonly ILogger<EmailSendingService> _logger;
    private readonly IAesEncryptionHelper _encryptionHelper;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ICustomGroupService _customGroupService;

    public EmailSendingService(
        ILogger<EmailSendingService> logger,
        IAesEncryptionHelper encryptionHelper,
        IEmailTemplateService emailTemplateService,
        ICustomGroupService customGroupService)
    {
        _logger = logger;
        _encryptionHelper = encryptionHelper;
        _emailTemplateService = emailTemplateService;
        _customGroupService = customGroupService;
    }

    public async Task SendBasicEmailsAsync(BasicEmailDetailsModel details)
    {
        var decryptedPassword = _encryptionHelper.Decrypt(details.AppPassword);
        var template = await _emailTemplateService.GetById(details.TemplateID);
        ArgumentNullException.ThrowIfNull(template);

        using var smtp = this.CreateSmtpClient(details.FromMail, decryptedPassword);
        var mailMessage = this.CreateMailMessage(
            details.FromMail,
            this.PopulateTemplate(template.Subject, details.CustomFields),
            this.PopulateTemplate(template.Body, details.CustomFields),
            details.Recipients,
            details.CC,
            details.Bcc);

        await this.SendEmailAsync(smtp, mailMessage);
    }

    public async Task SendPersonalizedEmailsAsync(PersonalizedEmailDetailsModel details)
    {
        var decryptedPassword = _encryptionHelper.Decrypt(details.AppPassword);
        var template = await _emailTemplateService.GetById(details.TemplateID);
        ArgumentNullException.ThrowIfNull(template);
        var group = await _customGroupService.GetById(details.GroupID);
        ArgumentNullException.ThrowIfNull(group);

        using var smtp = CreateSmtpClient(details.FromMail, decryptedPassword);

        var emailTasks = group.CustomFieldsList
            .Where(cf => cf.TryGetValue("Email", out string? toEmail) && !string.IsNullOrWhiteSpace(toEmail))
            .Select(cf =>
            {
                var mailMessage = this.CreateMailMessage(
                    details.FromMail,
                    this.PopulateTemplate(template.Subject, cf),
                    this.PopulateTemplate(template.Body, cf),
                    [cf["Email"].ToString()!],
                    details.CC,
                    details.Bcc
                );
                return this.SendEmailAsync(smtp, mailMessage);
            });

        await Task.WhenAll(emailTasks);
    }

    private SmtpClient CreateSmtpClient(string fromEmail, string decryptedPassword) => new()
    {
        Host = "smtp.gmail.com",
        EnableSsl = true,
        Credentials = new NetworkCredential(fromEmail, decryptedPassword),
        Port = 587
    };

    private MailMessage CreateMailMessage(string from, string subject, string body,
        IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc)
    {
        var mm = new MailMessage
        {
            From = new MailAddress(from),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        this.AddEmailsToAddressCollection(mm.To, to);
        this.AddEmailsToAddressCollection(mm.CC, cc);
        this.AddEmailsToAddressCollection(mm.Bcc, bcc);
        return mm;
    }

    private async Task SendEmailAsync(SmtpClient smtp, MailMessage mm)
    {
        try
        {
            await smtp.SendMailAsync(mm).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Error happens when sending emails from {mm.From} to {mm.To}: {ex.Message}");
        }
        finally
        {
            mm.Dispose();
        }
    }

    private string PopulateTemplate(string content, Dictionary<string, string> placeholders)
    {
        var sb = new StringBuilder(content);

        foreach (var kvp in placeholders)
        {
            sb.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
        }

        return sb.ToString();
    }

    private void AddEmailsToAddressCollection(MailAddressCollection collection, IEnumerable<string> emails)
    {
        if (emails != null)
        {
            foreach (var email in emails)
            {
                collection.Add(new MailAddress(email));
            }
        }
    }
}
