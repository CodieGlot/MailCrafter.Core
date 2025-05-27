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
    private readonly IEmailJobService _emailJobService;

    public EmailSendingService(
        ILogger<EmailSendingService> logger,
        IAesEncryptionHelper encryptionHelper,
        IEmailTemplateService emailTemplateService,
        ICustomGroupService customGroupService,
        IEmailJobService emailJobService)
    {
        _logger = logger;
        _encryptionHelper = encryptionHelper;
        _emailTemplateService = emailTemplateService;
        _customGroupService = customGroupService;
        _emailJobService = emailJobService;
    }

    public async Task SendBasicEmailsAsync(BasicEmailDetailsModel details)
    {
        try
        {
            // Update job status to Processing
            await _emailJobService.UpdateStatusAsync(details.JobId, "Processing");

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

            // Update job status to Completed
            await _emailJobService.UpdateStatusAsync(details.JobId, "Completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending basic emails");
            // Update job status to Failed
            await _emailJobService.UpdateStatusAsync(details.JobId, "Failed", ex.Message);
            throw;
        }
    }

    public async Task SendPersonalizedEmailsAsync(PersonalizedEmailDetailsModel details)
    {
        try
        {
            // Update job status to Processing
            await _emailJobService.UpdateStatusAsync(details.JobId, "Processing");

            var decryptedPassword = _encryptionHelper.Decrypt(details.AppPassword);
            var template = await _emailTemplateService.GetById(details.TemplateID);
            ArgumentNullException.ThrowIfNull(template);
            var group = await _customGroupService.GetById(details.GroupID);
            ArgumentNullException.ThrowIfNull(group);

            using var smtp = CreateSmtpClient(details.FromMail, decryptedPassword);

            // Process each email individually instead of in parallel
            foreach (var cf in group.CustomFieldsList.Where(cf => cf.TryGetValue("Email", out string? toEmail) && !string.IsNullOrWhiteSpace(toEmail)))
            {
                try
                {
                    var toEmail = cf["Email"].Trim();
                    _logger.LogInformation("Processing email: {Email}", toEmail);

                    var mailMessage = this.CreateMailMessage(
                        details.FromMail,
                        this.PopulateTemplate(template.Subject, cf),
                        this.PopulateTemplate(template.Body, cf),
                        new[] { toEmail },
                        details.CC,
                        details.Bcc
                    );

                    await this.SendEmailAsync(smtp, mailMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing email for recipient: {Email}", cf.GetValueOrDefault("Email", "unknown"));
                    // Continue with next email instead of failing the entire job
                }
            }

            // Update job status to Completed
            await _emailJobService.UpdateStatusAsync(details.JobId, "Completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending personalized emails");
            // Update job status to Failed
            await _emailJobService.UpdateStatusAsync(details.JobId, "Failed", ex.Message);
            throw;
        }
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
                if (!string.IsNullOrWhiteSpace(email))
                {
                    try
                    {
                        var trimmedEmail = email.Trim();
                        _logger.LogInformation("Adding email to collection: {Email}", trimmedEmail);
                        collection.Add(new MailAddress(trimmedEmail));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error adding email to collection: {Email}", email);
                        throw;
                    }
                }
            }
        }
    }
}
