namespace MailCrafter.Domain;
public class EmailAccount
{
    public EmailAccountStatus Status { get; set; }
    public string Email { get; set; }
    public string AppPassword { get; set; }
}
