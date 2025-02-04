namespace MailCrafter.Domain;
public class BasicEmailDetailsModel : EmailDetailsModel
{
    public List<string> Recipients { get; set; } = [];
}
public class PersonalizedEmailDetailsModel : EmailDetailsModel
{
    public string GroupID { get; set; } = string.Empty;
}
public class EmailDetailsModel
{
    public string TemplateID { get; set; } = string.Empty;
    public string FromMail { get; set; } = string.Empty;
    public string AppPassword { get; set; } = string.Empty;
    public List<string> CC { get; set; } = [];
    public List<string> Bcc { get; set; } = [];
}
