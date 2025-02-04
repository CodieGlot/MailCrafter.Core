namespace MailCrafter.Domain;
public class EmailFileInfo
{
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? ContentId { get; set; } // Only for inline images
}
