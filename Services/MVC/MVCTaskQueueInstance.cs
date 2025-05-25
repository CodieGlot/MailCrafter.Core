using MailCrafter.Domain;
using System.Text.Json;
using System.Threading.Channels;

namespace MailCrafter.Services;
public class MVCTaskQueueInstance
{
    private readonly Channel<WorkerTaskMessage> _queue = Channel.CreateUnbounded<WorkerTaskMessage>();

    public async Task EnqueueAsync(WorkerTaskMessage task)
    {
        await _queue.Writer.WriteAsync(task);
    }

    public async Task EnqueueAsync(string taskName, object payload)
    {
        await EnqueueAsync(new WorkerTaskMessage { TaskName = taskName, Payload = JsonSerializer.SerializeToElement(payload) });
    }

    public async Task<WorkerTaskMessage> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}
