using MailCrafter.Domain;

namespace MailCrafter.Services;
public interface ICustomGroupService
{
    Task<MongoInsertResult> Create(CustomGroupEntity groupEntity, bool setExpiration = false);
    Task<MongoDeleteResult> Delete(string id);
    Task<CustomGroupEntity?> GetById(string id);
    Task<MongoReplaceResult> Update(CustomGroupEntity groupEntity);
    Task<List<CustomGroupEntity>?> GetGroupsByUserId(string userId);
}