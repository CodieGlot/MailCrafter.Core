using MailCrafter.Domain;
using MailCrafter.Repositories;
using Microsoft.Extensions.Logging;

namespace MailCrafter.Services;
public class EmailScheduleService : IEmailScheduleService
{
    private readonly ILogger<EmailScheduleService> _logger;
    private readonly IEmailScheduleRepository _emailScheduleRepository;
    private readonly ITaskQueuePublisher _taskQueuePublisher;

    public EmailScheduleService(
        ILogger<EmailScheduleService> logger,
        IEmailScheduleRepository emailScheduleRepository,
        ITaskQueuePublisher taskQueuePublisher)
    {
        _logger = logger;
        _emailScheduleRepository = emailScheduleRepository;
        _taskQueuePublisher = taskQueuePublisher;
    }

    public async Task<MongoInsertResult> Create(EmailScheduleEntity entity)
    {
        return await _emailScheduleRepository.CreateAsync(entity);
    }

    public async Task<MongoDeleteResult> Delete(string id)
    {
        return await _emailScheduleRepository.DeleteAsync(id);
    }

    public async Task<EmailScheduleEntity?> GetById(string id)
    {
        return await _emailScheduleRepository.GetByIdAsync(id);
    }

    public async Task<List<EmailScheduleEntity>> GetByUserID(string userID)
    {
        return await _emailScheduleRepository.FindAsync(e => e.UserID == userID);
    }

    public async Task<MongoReplaceResult> Update(EmailScheduleEntity entity)
    {
        return await _emailScheduleRepository.ReplaceAsync(entity.ID, entity);
    }

    public async Task ProcessDueEmailsAsync()
    {
        var dueSchedules = await this.GetDueEmailSchedulesAsync();
        var idToValueMapping = new Dictionary<string, DateTime>();
        var idsToDelete = new List<string>();

        foreach (var schedule in dueSchedules)
        {
            _logger.LogInformation($"Begin to process email schedule: {schedule.ID}");

            var taskName = string.Empty;

            if (schedule.Details == null) continue;
            else if (schedule.Details is BasicEmailDetailsModel)
            {
                taskName = WorkerTaskNames.Send_Basic_Email;
            }
            else if (schedule.Details is PersonalizedEmailDetailsModel)
            {
                taskName = WorkerTaskNames.Send_Personailized_Email;
            }

            await _taskQueuePublisher.PublishMessageAsync(taskName, schedule.Details);

            if (schedule.Recurrence == RecurrencePattern.OneTime)
            {
                idsToDelete.Add(schedule.ID);
            }
            else
            {
                idToValueMapping[schedule.ID] = schedule.CalculateNextRunTime();
            }

            if (idsToDelete.Count != 0)
            {
                await _emailScheduleRepository.DeleteManyAsync(idsToDelete);
            }

            if (idToValueMapping.Count != 0)
            {
                await _emailScheduleRepository.UpdateManyAsync(idToValueMapping, x => x.NextSendTime);
            }

            _logger.LogInformation($"Finish processing email schedule: {schedule.ID}");
        }
    }
    public async Task<List<EmailScheduleEntity>> GetDueEmailSchedulesAsync()
    {
        return await _emailScheduleRepository.FindAsync(s => s.NextSendTime <= DateTime.UtcNow);
    }

    public async Task<List<EmailScheduleEntity>> GetPageQueryDataAsync(PageQueryDTO<EmailScheduleEntity> queryDTO)
    {
        return await _emailScheduleRepository.GetPageQueryDataAsync(queryDTO);
    }
}
