namespace MailCrafter.Domain;
public abstract class EmailDetailsAbstractModel
{
    public string JobId { get; set; } = string.Empty;
    public string TemplateID { get; set; } = string.Empty;
    public string FromMail { get; set; } = string.Empty;
    public string AppPassword { get; set; } = string.Empty;
    public List<string> CC { get; set; } = [];
    public List<string> Bcc { get; set; } = [];
}
