namespace MailCrafter.Domain;
public class EmailScheduleEntity : MongoEntityBase
{
    public string UserID { get; set; } = string.Empty;
    public EmailDetailsAbstractModel? Details { get; set; }
    public DateTime NextSendTime { get; set; }
    public RecurrencePattern Recurrence { get; set; }
    public DateTime CalculateNextRunTime()
    {
        return Recurrence switch
        {
            RecurrencePattern.Daily => NextSendTime.AddDays(1),
            RecurrencePattern.Weekly => NextSendTime.AddDays(7),
            RecurrencePattern.Hourly => NextSendTime.AddHours(1),
            _ => DateTime.MaxValue
        };
    }
}
public enum RecurrencePattern
{
    OneTime = 0,
    Hourly = 1,
    Daily = 2,
    Weekly = 3,
}
