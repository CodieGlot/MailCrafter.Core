using MailCrafter.Domain;
using System.Linq.Expressions;

namespace MailCrafter.Repositories;
public abstract class MongoCollectionRepostioryBase<T> : IMongoCollectionRepostioryBase<T> where T : MongoEntityBase
{
    protected readonly string _collectionName;
    protected readonly IMongoDBRepository _mongoDBRepository;

    protected MongoCollectionRepostioryBase(string collectionName, IMongoDBRepository mongoDBRepository)
    {
        _collectionName = collectionName;
        _mongoDBRepository = mongoDBRepository;
    }
    public async Task<T?> GetByIdAsync(string id)
    {
        return await _mongoDBRepository.GetByIdAsync<T>(id, _collectionName);
    }
    public async Task<T?> GetByPropertyAsync(Expression<Func<T, bool>> filter)
    {
        return await _mongoDBRepository.GetByPropertyAsync(filter, _collectionName);
    }
    public async Task<MongoInsertResult> CreateAsync(T entity)
    {
        return await _mongoDBRepository.CreateAsync(entity, _collectionName);
    }
    public async Task<MongoUpdateResult> UpdateFieldAsync<TField>(string id, Expression<Func<T, TField>> fieldSelector, TField value)
    {
        return await _mongoDBRepository.UpdateFieldAsync(id, fieldSelector, value, _collectionName);
    }
    public async Task<MongoReplaceResult> ReplaceAsync(string id, T entity)
    {
        return await _mongoDBRepository.ReplaceAsync(id, entity, _collectionName);
    }
    public async Task<MongoDeleteResult> DeleteAsync(string id)
    {
        return await _mongoDBRepository.DeleteAsync<T>(id, _collectionName);
    }
    public async Task<TField?> GetFieldValueAsync<TField>(string id, Expression<Func<T, TField>> fieldSelector)
    {
        return await _mongoDBRepository.GetFieldValueAsync(id, fieldSelector, _collectionName);
    }
    public async Task<TResult?> GetFieldValueInArrayAsync<TItem, TResult>(
        string id,
        Expression<Func<T, IEnumerable<TItem>>> arraySelector,
        Expression<Func<TItem, TResult>> itemSelector,
        Expression<Func<TItem, bool>> identifierFilter)
    {
        return await _mongoDBRepository.GetFieldValueInArrayAsync(id, arraySelector, itemSelector, identifierFilter, _collectionName);
    }
    public async Task<List<TResult>> GetFieldValuesInArrayAsync<TItem, TResult>(
        string id,
        Expression<Func<T, IEnumerable<TItem>>> arraySelector,
        Expression<Func<TItem, TResult>> itemSelector)
    {
        return await _mongoDBRepository.GetFieldValuesInArrayAsync(id, arraySelector, itemSelector, _collectionName);
    }
    public async Task<MongoUpdateResult> AddToArrayAsync<TItem>(string id, Expression<Func<T, IEnumerable<TItem>>> arrayField, TItem newItem)
    {
        return await _mongoDBRepository.AddToArrayAsync(id, arrayField, newItem, _collectionName);
    }
    public async Task<MongoUpdateResult> RemoveFromArrayAsync<TItem>(string id, Expression<Func<T, IEnumerable<TItem>>> arrayField, Expression<Func<TItem, bool>> itemSelector)
    {
        return await _mongoDBRepository.RemoveFromArrayAsync(id, arrayField, itemSelector, _collectionName);
    }
    public async Task<MongoUpdateResult> UpdateFieldInArrayWithConditionAsync<TItem, TValue>(
        string id,
        Expression<Func<T, IEnumerable<TItem>>> arraySelector,
        Expression<Func<TItem, TValue>> fieldSelector,
        TValue value,
        Expression<Func<TItem, bool>> identifierFilter)
        where TItem : class
    {
        return await _mongoDBRepository.UpdateFieldInArrayWithConditionAsync(id, arraySelector, fieldSelector, value, identifierFilter, _collectionName);
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> filter)
    {
        return await _mongoDBRepository.FindAsync(filter, _collectionName);
    }

    public async Task<MongoUpdateResult> UpdateManyAsync<TField>(
        List<string> ids,
        Expression<Func<T, TField>> fieldSelector,
        TField value)
    {
        return await _mongoDBRepository.UpdateManyAsync(ids, fieldSelector, value, _collectionName);
    }

    public async Task<MongoBulkWriteResult> UpdateManyAsync<TField>(
        Dictionary<string, TField> idToValueMapper,
        Expression<Func<T, TField>> fieldSelector)
    {
        return await _mongoDBRepository.UpdateManyAsync(idToValueMapper, fieldSelector, _collectionName);
    }

    public async Task<MongoDeleteResult> DeleteManyAsync(List<string> ids)
    {
        return await _mongoDBRepository.DeleteManyAsync<T>(ids, _collectionName);
    }

    public async Task<List<T>> GetPageQueryDataAsync(PageQueryDTO<T> queryDTO)
    {
        return await _mongoDBRepository.GetPageQueryDataAsync(queryDTO);
    }
}
