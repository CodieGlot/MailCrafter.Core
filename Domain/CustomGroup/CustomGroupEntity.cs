namespace MailCrafter.Domain;
public class CustomGroupEntity : TimeTrackedEntity
{
    public string UserID { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public List<Dictionary<string, string>> CustomFields { get; set; } = [];
}
