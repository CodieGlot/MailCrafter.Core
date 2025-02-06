using System.Text.Json;

namespace MailCrafter.Domain;
public class WorkerTaskMessage
{
    public string TaskName { get; set; } = string.Empty;
    public JsonElement Payload { get; set; }
}