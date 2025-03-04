using MailCrafter.Domain;

namespace MailCrafter.Services;
public interface ICustomGroupService : IBasicOperations<CustomGroupEntity>
{
    Task<List<CustomGroupEntity>> GetGroupsByUserId(string userId);
    Task<List<CustomGroupEntity>> GetPageQueryDataAsync(PageQueryDTO queryDTO);
}