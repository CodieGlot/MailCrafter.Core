namespace MailCrafter.Domain;
public class BasicEmailDetailsModel : EmailDetailsAbstractModel
{
    public string JobId { get; set; } = string.Empty;
    public List<string> Recipients { get; set; } = [];
    public Dictionary<string, string> CustomFields { get; set; } = [];
}
