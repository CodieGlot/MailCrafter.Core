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
    public async Task<MongoInsertResult> Create(CustomGroupEntity entity)
    {
        return await _customGroupRepository.CreateAsync(entity);
    }
    public async Task<MongoDeleteResult> Delete(string id)
    {
        return await _customGroupRepository.DeleteAsync(id);
    }
    public async Task<CustomGroupEntity?> GetById(string id)
    {
        return await _customGroupRepository.GetByIdAsync(id);
    }

    public async Task<List<CustomGroupEntity>> GetGroupsByUserId(string userId)
    {
        return await _customGroupRepository.FindAsync(group => group.UserID == userId);
    }

    public async Task<List<CustomGroupEntity>> GetPageQueryDataAsync(PageQueryDTO<CustomGroupEntity> queryDTO)
    {
        return await _customGroupRepository.GetPageQueryDataAsync(queryDTO);
    }

    public async Task<MongoReplaceResult> Update(CustomGroupEntity entity)
    {
        return await _customGroupRepository.ReplaceAsync(entity.ID, entity);
    }
}
