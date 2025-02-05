using System.Text.Json;

namespace MailCrafter.Domain;
public class WorkerTaskMessage
{
    public string TaskName { get; set; } = string.Empty;
    public JsonElement Payload { get; set; }
}

public static class WorkerTaskNames
{
    public const string Send_Basic_Email = "Send_Basic_Email";
    public const string Send_Personailized_Email = "Send_Personailized_Email";
}