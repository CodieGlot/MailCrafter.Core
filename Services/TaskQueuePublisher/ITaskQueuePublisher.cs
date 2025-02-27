namespace MailCrafter.Services;

public interface ITaskQueuePublisher
{
    Task PublishMessageAsync(string taskName, object payload);
}