using MailCrafter.Domain;
using MailCrafter.Utils.Helpers;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace MailCrafter.Services;
public class EmailService
{
    private readonly IAesEncryptionHelper _encryptionHelper;
    private readonly IEmailTemplateService _emailTemplateService;
    public EmailService(IAesEncryptionHelper encryptionHelper, IEmailTemplateService emailTemplateService)
    {
        _encryptionHelper = encryptionHelper;
        _emailTemplateService = emailTemplateService;
    }
    public async Task SendBasicEmailsAsync(BasicEmailDetailsModel details)
    {
        var decryptedPassword = _encryptionHelper.Decrypt(details.AppPassword);
        var template = await _emailTemplateService.GetById(details.TemplateID);
        ArgumentNullException.ThrowIfNull(template);
        using (var mm = new MailMessage())
        {
            mm.From = new MailAddress(details.FromMail);
            this.AddEmailsToAddressCollection(mm.To, details.Recipients);
            this.AddEmailsToAddressCollection(mm.CC, details.CC);
            this.AddEmailsToAddressCollection(mm.Bcc, details.Bcc);
            //mm.Subject = this.PopulateTemplate(template.Subject, details.Placeholders);
            //mm.Body = this.PopulateTemplate(template.Body, details.Placeholders);
            mm.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                var networkCred = new NetworkCredential(details.FromMail, decryptedPassword);
                smtp.Credentials = networkCred;
                smtp.Port = 587;
                try
                {
                    await smtp.SendMailAsync(mm).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // Log error here
                }
            }
        }
    }
    public async Task SendPersonalizedEmailsAsync(PersonalizedEmailDetailsModel details)
    {

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
    private void AddEmailsToAddressCollection(MailAddressCollection collection, List<string> emails)
    {
        if (emails != null)
        {
            emails.ForEach(email => collection.Add(new MailAddress(email)));
        }
    }
}
