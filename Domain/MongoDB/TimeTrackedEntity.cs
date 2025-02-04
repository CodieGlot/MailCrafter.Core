namespace MailCrafter.Domain;
public class TimeTrackedEntity : MongoEntityBase
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public DateTime? ExpiresAt { get; set; }
}
