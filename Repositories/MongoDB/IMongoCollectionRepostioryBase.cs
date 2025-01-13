using MailCrafter.Domain;
using System.Linq.Expressions;

namespace MailCrafter.Repositories;
public interface IMongoCollectionRepostioryBase<T> where T : MongoEntityBase
{
    Task<MongoUpdateResult> AddToArrayAsync<TItem>(string id, Expression<Func<T, IEnumerable<TItem>>> arrayField, TItem newItem);
    Task<MongoInsertResult> CreateAsync(T entity);
    Task<MongoDeleteResult> DeleteAsync(string id);
    Task<T?> GetByIdAsync(string id);
    Task<T?> GetByPropertyAsync(Expression<Func<T, bool>> filter);
    Task<TField?> GetFieldValueAsync<TField>(string id, Expression<Func<T, TField>> fieldSelector);
    Task<TResult?> GetFieldValueInArrayAsync<TItem, TResult>(string id, Expression<Func<T, IEnumerable<TItem>>> arraySelector, Expression<Func<TItem, TResult>> itemSelector, Expression<Func<TItem, bool>> identifierFilter);
    Task<List<TResult>> GetFieldValuesInArrayAsync<TItem, TResult>(string id, Expression<Func<T, IEnumerable<TItem>>> arraySelector, Expression<Func<TItem, TResult>> itemSelector);
    Task<MongoUpdateResult> RemoveFromArrayAsync<TItem>(string id, Expression<Func<T, IEnumerable<TItem>>> arrayField, Expression<Func<TItem, bool>> itemSelector);
    Task<MongoReplaceResult> ReplaceAsync(string id, T entity);
    Task<MongoUpdateResult> UpdateFieldAsync<TField>(string id, Expression<Func<T, TField>> fieldSelector, TField value);
    Task<MongoUpdateResult> UpdateFieldInArrayWithConditionAsync<TItem, TValue>(string id, Expression<Func<T, IEnumerable<TItem>>> arraySelector, Expression<Func<TItem, TValue>> fieldSelector, TValue value, Expression<Func<TItem, bool>> identifierFilter) where TItem : class;
}