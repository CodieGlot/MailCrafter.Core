using MailCrafter.Domain;
using MailCrafter.Utils.Helpers;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;
using HtmlAgilityPack;

namespace MailCrafter.Services;
public class EmailSendingService : IEmailSendingService
{
    private readonly ILogger<EmailSendingService> _logger;
    private readonly IAesEncryptionHelper _encryptionHelper;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ICustomGroupService _customGroupService;
    private readonly IEmailJobService _emailJobService;
    private readonly IEmailTrackingService _emailTrackingService;

    public EmailSendingService(
        ILogger<EmailSendingService> logger,
        IAesEncryptionHelper encryptionHelper,
        IEmailTemplateService emailTemplateService,
        ICustomGroupService customGroupService,
        IEmailJobService emailJobService,
        IEmailTrackingService emailTrackingService)
    {
        _logger = logger;
        _encryptionHelper = encryptionHelper;
        _emailTemplateService = emailTemplateService;
        _customGroupService = customGroupService;
        _emailJobService = emailJobService;
        _emailTrackingService = emailTrackingService;
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
                details.Bcc,
                details.JobId);

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

            using var smtp = this.CreateSmtpClient(details.FromMail, decryptedPassword);

            // Group custom fields by email address
            var emailGroups = group.CustomFieldsList
                .Where(cf => cf.TryGetValue("Email", out string? email) && !string.IsNullOrWhiteSpace(email))
                .GroupBy(cf => cf["Email"].Trim())
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var emailGroup in emailGroups)
            {
                try
                {
                    var toEmail = emailGroup.Key;

                    // Create a dictionary for template replacement using fieldName as key
                    var templateFields = new Dictionary<string, string>();
                    
                    // Combine all fields for this email
                    foreach (var cf in emailGroup.Value)
                    {
                        var fieldName = cf.GetValueOrDefault("fieldName", "").Trim();
                        var fieldValue = cf.GetValueOrDefault("fieldValue", "").Trim();
                        if (!string.IsNullOrEmpty(fieldName))
                        {
                            templateFields[fieldName] = fieldValue;
                        }
                    }

                    _logger.LogInformation("Template fields for {Email}: {Fields}", toEmail, 
                        string.Join(", ", templateFields.Select(kv => $"{kv.Key}={kv.Value}")));

                    var mailMessage = this.CreateMailMessage(
                        details.FromMail,
                        this.PopulateTemplate(template.Subject, templateFields),
                        this.PopulateTemplate(template.Body, templateFields),
                        new[] { toEmail },
                        details.CC,
                        details.Bcc,
                        details.JobId);

                    await this.SendEmailAsync(smtp, mailMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing email for recipient: {Email}", emailGroup.Key);
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

    private MailMessage CreateMailMessage(string fromEmail, string subject, string body, IEnumerable<string> recipients, IEnumerable<string> cc, IEnumerable<string> bcc, string jobId)
    {
        var message = new MailMessage
        {
            From = new MailAddress(fromEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        this.AddEmailsToAddressCollection(message.To, recipients);
        this.AddEmailsToAddressCollection(message.CC, cc);
        this.AddEmailsToAddressCollection(message.Bcc, bcc);

        // Add tracking pixel
        var trackingPixel = _emailTrackingService.GenerateTrackingPixel(jobId, recipients.First());
        _logger.LogInformation("Generated tracking pixel URL: {Url}", trackingPixel);
        var trackingPixelHtml = $"<img src='{trackingPixel}' width='1' height='1' style='display:none' />";
        
        // Check if body has </body> tag
        if (message.Body.Contains("</body>"))
        {
            message.Body = message.Body.Replace("</body>", $"{trackingPixelHtml}</body>");
        }
        else
        {
            // If no body tag, wrap the content in body tags
            message.Body = $"{message.Body}<body>{trackingPixelHtml}</body>";
        }
        _logger.LogInformation("Added tracking pixel to email body");

        // Replace links with tracking links
        var htmlDocument = new HtmlAgilityPack.HtmlDocument();
        htmlDocument.LoadHtml(message.Body);
        var links = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
        if (links != null)
        {
            _logger.LogInformation("Found {Count} links to replace with tracking links", links.Count);
            foreach (var link in links)
            {
                var originalUrl = link.GetAttributeValue("href", "");
                if (!string.IsNullOrEmpty(originalUrl) && !originalUrl.StartsWith("#"))
                {
                    var trackingUrl = _emailTrackingService.GenerateTrackingLink(originalUrl, jobId, recipients.First());
                    _logger.LogInformation("Replacing link {OriginalUrl} with tracking link {TrackingUrl}", originalUrl, trackingUrl);
                    link.SetAttributeValue("href", trackingUrl);
                }
            }
            message.Body = htmlDocument.DocumentNode.OuterHtml;
        }
        else
        {
            _logger.LogInformation("No links found in email body to replace");
        }

        return message;
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
