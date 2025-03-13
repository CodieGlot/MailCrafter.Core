namespace MailCrafter.Domain;
public class EmailTemplateEntity : TimeTrackedEntity
{
    public string UserID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<string> Placeholders { get; set; } = [];
    public List<EmailFileInfo> Attachments { get; set; } = [];
    public List<EmailFileInfo> InlineImages { get; set; } = [];
}
public class EmailFileInfo
{
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? ContentId { get; set; } // Only for inline images
}