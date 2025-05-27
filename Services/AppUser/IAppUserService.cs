using MailCrafter.Domain;

namespace MailCrafter.Services;

public interface IAppUserService : IBasicOperations<AppUserEntity>
{
    Task<MongoUpdateResult> AddEmailAccount(string id, EmailAccount emailAccount);
    Task<AppUserEntity?> GetByUsernameOrEmail(string usernameOrEmail);
    Task<List<string>> GetEmailAccountsOfUser(string id);
    Task<MongoUpdateResult> RemoveEmailAccount(string id, string email);
    Task<MongoUpdateResult> UpdateEmailPassword(string id, string email, string newPassword);
    Task<List<AppUserEntity>> GetPageQueryDataAsync(PageQueryDTO queryDTO);
    Task<AppUserEntity?> GetById(string id);
    Task<MongoInsertResult> Create(AppUserEntity user);
    Task<MongoUpdateResult> Update(AppUserEntity user);
    Task<MongoDeleteResult> Delete(string id);
}
