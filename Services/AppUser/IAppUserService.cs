using MailCrafter.Domain;

namespace MailCrafter.Services;

public interface IAppUserService : IBasicOperations<AppUserEntity>
{
    Task<List<AppUserEntity?>> GetAllAppUsers();
    Task<MongoUpdateResult> AddEmailAccount(string id, EmailAccount emailAccount);
    Task<AppUserEntity?> GetByUsernameOrEmail(string usernameOrEmail);
    Task<List<string>> GetEmailAccountsOfUser(string id);
    Task<MongoUpdateResult> RemoveEmailAccount(string id, string email);
    Task<MongoUpdateResult> UpdateEmailPassword(string id, string email, string newPassword);
}
