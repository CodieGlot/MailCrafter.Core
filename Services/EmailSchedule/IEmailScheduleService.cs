using MailCrafter.Domain;

namespace MailCrafter.Services;
public interface IEmailScheduleService : IBasicOperations<EmailScheduleEntity>
{
    Task<List<EmailScheduleEntity>> GetByUserID(string userID);
    Task<List<EmailScheduleEntity>> GetDueEmailSchedulesAsync();
    Task ProcessDueEmailsAsync();
}
