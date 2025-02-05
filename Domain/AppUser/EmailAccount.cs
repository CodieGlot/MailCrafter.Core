namespace MailCrafter.Domain;
public class EmailAccount
{
    public EmailAccountStatus Status { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public string AppPassword { get; set; } = string.Empty;
}
