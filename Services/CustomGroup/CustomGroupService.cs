using MailCrafter.Domain;
using MailCrafter.Repositories;

namespace MailCrafter.Services;
public class CustomGroupService : ICustomGroupService
{
    private readonly ICustomGroupRepository _customGroupRepository;
    public CustomGroupService(ICustomGroupRepository customGroupRepository)
    {
        _customGroupRepository = customGroupRepository;
    }
    public async Task<MongoInsertResult> Create(CustomGroupEntity groupEntity, bool setExpiration = false)
    {
        groupEntity.ExpiresAt = setExpiration ? groupEntity.CreatedAt.AddHours(1) : null;
        return await _customGroupRepository.CreateAsync(groupEntity);
    }
    public async Task<MongoDeleteResult> Delete(string id)
    {
        return await _customGroupRepository.DeleteAsync(id);
    }
    public async Task<CustomGroupEntity?> GetById(string id)
    {
        return await _customGroupRepository.GetByIdAsync(id);
    }

    public async Task<List<CustomGroupEntity>?> GetGroupsByUserId(string userId)
    {
        return await _customGroupRepository.FindAsync(group => group.UserID == userId);
    }

    public async Task<MongoReplaceResult> Update(CustomGroupEntity groupEntity)
    {
        return await _customGroupRepository.ReplaceAsync(groupEntity.ID, groupEntity);
    }
}
