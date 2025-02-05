namespace MailCrafter.Domain;

public class EmailDetailsDto
{
    public string FromMail { get; set; } = string.Empty;
    public string AppPassword { get; set; } = string.Empty;
    public List<string> Recipients { get; set; } = new();
    public List<string> CC { get; set; } = new();
    public List<string> Bcc { get; set; } = new();
    public Dictionary<string, string> Placeholders { get; set; } = new();
}
