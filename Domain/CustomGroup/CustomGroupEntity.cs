namespace MailCrafter.Domain;
public class CustomGroupEntity : TimeTrackedEntity
{
    public string UserID { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public List<string> PropertyList { get; set; } = [];
    // Ensure there's an Email property in each custom field
    public List<Dictionary<string, string>> CustomFieldsList { get; set; } = [];
}
