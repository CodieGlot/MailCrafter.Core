namespace MailCrafter.Domain;
public class PersonalizedEmailDetailsModel : EmailDetailsAbstractModel
{
    public string JobId { get; set; } = string.Empty;
    public string GroupID { get; set; } = string.Empty;
}
