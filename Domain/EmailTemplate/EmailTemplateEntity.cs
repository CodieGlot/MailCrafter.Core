namespace MailCrafter.Domain;
public class EmailTemplateEntity : MongoEntityBase
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<string> Placeholders { get; set; } = new();
    public List<EmailFileInfo> Attachments { get; set; } = new();
    public List<EmailFileInfo> InlineImages { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}