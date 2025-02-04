namespace MailCrafter.Domain;
public class AppUserEntity : MongoEntityBase
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<EmailAccount> EmailAccounts { get; set; } = new();
}
