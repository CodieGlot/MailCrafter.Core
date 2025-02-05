using System.Text.Json;

namespace MailCrafter.Domain;
public class WorkerTaskMessage
{
    public string TaskName { get; set; } = string.Empty;
    public JsonElement Payload { get; set; }
}

public static class WorkerTaskNames
{
    public const string Send_Email = "Send_Email";
}