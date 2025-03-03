using MailCrafter.Domain;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace MailCrafter.Repositories;
public interface IMongoDBRepository
{
    Task<MongoUpdateResult> AddToArrayAsync<T, TItem>(string id, Expression<Func<T, IEnumerable<TItem>>> arrayField, TItem newItem, string collectionName) where T : MongoEntityBase;
    Task<MongoInsertResult> CreateAsync<T>(T entity, string collectionName) where T : MongoEntityBase;
    Task<MongoDeleteResult> DeleteAsync<T>(string id, string collectionName) where T : MongoEntityBase;
    Task<MongoDeleteResult> DeleteManyAsync<T>(List<string> ids, string collectionName) where T : MongoEntityBase;
    Task<List<T>> FindAsync<T>(Expression<Func<T, bool>> filter, string collectionName);
    Task<T?> GetByIdAsync<T>(string id, string collectionName) where T : MongoEntityBase;
    Task<T?> GetByPropertyAsync<T>(Expression<Func<T, bool>> filter, string collectionName);
    Task<TField?> GetFieldValueAsync<T, TField>(string id, Expression<Func<T, TField>> fieldSelector, string collectionName) where T : MongoEntityBase;
    Task<TResult?> GetFieldValueInArrayAsync<T, TItem, TResult>(string id, Expression<Func<T, IEnumerable<TItem>>> arraySelector, Expression<Func<TItem, TResult>> itemSelector, Expression<Func<TItem, bool>> identifierFilter, string collectionName) where T : MongoEntityBase;
    Task<List<TResult>> GetFieldValuesInArrayAsync<T, TItem, TResult>(string id, Expression<Func<T, IEnumerable<TItem>>> arraySelector, Expression<Func<TItem, TResult>> itemSelector, string collectionName) where T : MongoEntityBase;
    Task<List<T>> GetPageQueryDataAsync<T>(PageQueryDTO<T> queryDTO);
    IMongoQueryable<T> GetQueryable<T>(string collectionName);
    Task<MongoUpdateResult> RemoveFromArrayAsync<T, TItem>(string id, Expression<Func<T, IEnumerable<TItem>>> arrayField, Expression<Func<TItem, bool>> itemSelector, string collectionName) where T : MongoEntityBase;
    Task<MongoReplaceResult> ReplaceAsync<T>(string id, T entity, string collectionName) where T : MongoEntityBase;
    Task<MongoUpdateResult> UpdateFieldAsync<T, TField>(string id, Expression<Func<T, TField>> fieldSelector, TField value, string collectionName) where T : MongoEntityBase;
    Task<MongoUpdateResult> UpdateFieldInArrayWithConditionAsync<T, TItem, TValue>(string id, Expression<Func<T, IEnumerable<TItem>>> arraySelector, Expression<Func<TItem, TValue>> fieldSelector, TValue value, Expression<Func<TItem, bool>> identifierFilter, string collectionName)
        where T : MongoEntityBase
        where TItem : class;
    Task<MongoUpdateResult> UpdateFieldsAsync<T>(string id, Dictionary<string, object> updates, string collectionName) where T : MongoEntityBase;
    Task<MongoUpdateResult> UpdateManyAsync<T, TField>(List<string> ids, Expression<Func<T, TField>> fieldSelector, TField value, string collectionName) where T : MongoEntityBase;
    Task<MongoBulkWriteResult> UpdateManyAsync<T, TField>(Dictionary<string, TField> idToValueMapper, Expression<Func<T, TField>> fieldSelector, string collectionName) where T : MongoEntityBase;
}