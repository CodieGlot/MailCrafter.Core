namespace MailCrafter.Domain;
public class BasicEmailDetailsModel : EmailDetailsAbstractModel
{
    public List<string> Recipients { get; set; } = [];
    public Dictionary<string, string> CustomFields { get; set; } = [];
}
