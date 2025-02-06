using MailCrafter.Domain;

namespace MailCrafter.Services;

public interface ITaskQueuePublisher
{
    Task PublishMessageAsync(WorkerTaskMessage message);
}