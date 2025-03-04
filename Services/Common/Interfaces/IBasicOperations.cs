using MailCrafter.Domain;

namespace MailCrafter.Services;

public interface IBasicOperations<T>
{
    Task<MongoInsertResult> Create(T entity);
    Task<MongoDeleteResult> Delete(string id);
    Task<T?> GetById(string id);
    Task<MongoReplaceResult> Update(T entity);
}
