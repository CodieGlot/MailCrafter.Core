﻿using MailCrafter.Domain;
using MailCrafter.Utils.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace MailCrafter.Repositories;

public class MongoDBRepository : IMongoDBRepository
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<MongoDBRepository> _logger;

    public MongoDBRepository(IMongoClient mongoClient, IConfiguration configuration, ILogger<MongoDBRepository> logger)
    {
        _database = mongoClient.GetDatabase(configuration["MongoDB:DatabaseName"]);
        _logger = logger;
    }

    public IMongoQueryable<T> GetQueryable<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName).AsQueryable();
    }

    public async Task<T?> GetByIdAsync<T>(string id, string collectionName)
        where T : MongoEntityBase
    {
        return await this.GetByPropertyAsync<T>(x => x.ID == id, collectionName);
    }
    public async Task<T?> GetByPropertyAsync<T>(Expression<Func<T, bool>> filter, string collectionName)
    {
        return await _database.GetCollection<T>(collectionName)
                              .Find(filter)
                              .FirstOrDefaultAsync();
    }
    public async Task<List<T>> FindAsync<T>(Expression<Func<T, bool>> filter, string collectionName)
    {
        return await _database.GetCollection<T>(collectionName).Find(filter).ToListAsync();
    }

    public async Task<MongoInsertResult> CreateAsync<T>(T entity, string collectionName)
        where T : MongoEntityBase
    {
        try
        {
            _logger.LogInformation("Inserting document into collection {CollectionName}", collectionName);
            await _database.GetCollection<T>(collectionName).InsertOneAsync(entity);
            _logger.LogInformation("Document inserted successfully with ID: {DocumentId}", entity.ID);
            return new MongoInsertResult(true, entity.ID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting document into collection {CollectionName}", collectionName);
            return new MongoInsertResult(false, entity.ID);
        }
    }

    public async Task<MongoReplaceResult> ReplaceAsync<T>(string id, T entity, string collectionName) where T : MongoEntityBase
    {
        var result = await _database.GetCollection<T>(collectionName)
                                    .ReplaceOneAsync(x => x.ID == id, entity);
        return new MongoReplaceResult(result);
    }

    public async Task<MongoDeleteResult> DeleteAsync<T>(string id, string collectionName)
        where T : MongoEntityBase
    {
        var result = await _database.GetCollection<T>(collectionName)
                                    .DeleteOneAsync(x => x.ID == id);
        return new MongoDeleteResult(result);
    }

    public async Task<MongoDeleteResult> DeleteManyAsync<T>(List<string> ids, string collectionName) where T : MongoEntityBase
    {
        var filter = Builders<T>.Filter.In(e => e.ID, ids);
        var result = await _database.GetCollection<T>(collectionName).DeleteManyAsync(filter);

        return new MongoDeleteResult(result);
    }

    public async Task<MongoUpdateResult> UpdateManyAsync<T, TField>(
        List<string> ids, Expression<Func<T, TField>> fieldSelector,
        TField value, string collectionName)
        where T : MongoEntityBase
    {
        var filter = Builders<T>.Filter.In(e => e.ID, ids);
        var update = Builders<T>.Update.Set(fieldSelector, value);

        var result = await _database.GetCollection<T>(collectionName).UpdateManyAsync(filter, update);
        return new MongoUpdateResult(result);
    }

    public async Task<MongoBulkWriteResult> UpdateManyAsync<T, TField>(
        Dictionary<string, TField> idToValueMapper,
        Expression<Func<T, TField>> fieldSelector,
        string collectionName)
        where T : MongoEntityBase
    {
        var bulkOps = new List<WriteModel<T>>();

        foreach (var (id, value) in idToValueMapper)
        {
            var filter = Builders<T>.Filter.Eq(e => e.ID, id);
            var update = Builders<T>.Update.Set(fieldSelector, value);
            bulkOps.Add(new UpdateOneModel<T>(filter, update));
        }

        if (bulkOps.Any())
        {
            var result = await _database.GetCollection<T>(collectionName).BulkWriteAsync(bulkOps);
            return new MongoBulkWriteResult(result);
        }

        return new MongoBulkWriteResult(isAcknowledged: false, isSuccessful: false);
    }

    public async Task<MongoUpdateResult> UpdateFieldAsync<T, TField>(
        string id, Expression<Func<T, TField>> fieldSelector,
        TField value, string collectionName)
        where T : MongoEntityBase
    {
        var filter = Builders<T>.Filter.Eq(x => x.ID, id);

        var fieldName = fieldSelector.GetMemberName();
        var update = Builders<T>.Update.Set(fieldName, value);

        var result = await _database.GetCollection<T>(collectionName)
                                    .UpdateOneAsync(filter, update);

        return new MongoUpdateResult(result);
    }

    public async Task<MongoUpdateResult> UpdateFieldsAsync<T>(string id, Dictionary<string, object> updates, string collectionName) where T : MongoEntityBase
    {
        var filter = Builders<T>.Filter.Eq(x => x.ID, id);
        var updateDefinitions = new List<UpdateDefinition<T>>();

        foreach (var update in updates)
        {
            var updateDefinition = Builders<T>.Update.Set(update.Key, update.Value);
            updateDefinitions.Add(updateDefinition);
        }

        var combinedUpdate = Builders<T>.Update.Combine(updateDefinitions);

        var result = await _database.GetCollection<T>(collectionName)
                                    .UpdateOneAsync(filter, combinedUpdate);

        return new MongoUpdateResult(result);
    }

    public async Task<TField?> GetFieldValueAsync<T, TField>(string id, Expression<Func<T, TField>> fieldSelector, string collectionName) where T : MongoEntityBase
    {
        var filter = Builders<T>.Filter.Eq(x => x.ID, id);

        var fieldName = fieldSelector.GetMemberName();
        var projection = Builders<T>.Projection.Exclude("_id").Include(fieldName);

        var result = await _database.GetCollection<T>(collectionName)
                                    .Find(filter)
                                    .Project(projection)
                                    .FirstOrDefaultAsync();

        return this.GetFieldValueFromBsonDocument<TField>(result, fieldName);
    }

    public async Task<MongoUpdateResult> AddToArrayAsync<T, TItem>(
        string id,
        Expression<Func<T, IEnumerable<TItem>>> arrayField,
        TItem newItem,
        string collectionName)
        where T : MongoEntityBase
    {
        var filter = Builders<T>.Filter.Eq(x => x.ID, id);
        var update = Builders<T>.Update.Push(arrayField, newItem);

        var result = await _database.GetCollection<T>(collectionName)
                                    .UpdateOneAsync(filter, update);

        return new MongoUpdateResult(result);
    }

    public async Task<MongoUpdateResult> RemoveFromArrayAsync<T, TItem>(
        string id,
        Expression<Func<T, IEnumerable<TItem>>> arrayField,
        Expression<Func<TItem, bool>> itemSelector,
        string collectionName)
        where T : MongoEntityBase
    {
        var filter = Builders<T>.Filter.Eq(x => x.ID, id);
        var update = Builders<T>.Update.PullFilter(arrayField, itemSelector);

        var result = await _database.GetCollection<T>(collectionName)
                                    .UpdateOneAsync(filter, update);

        return new MongoUpdateResult(result);
    }

    public async Task<TResult?> GetFieldValueInArrayAsync<T, TItem, TResult>(
        string id,
        Expression<Func<T, IEnumerable<TItem>>> arraySelector,
        Expression<Func<TItem, TResult>> itemSelector,
        Expression<Func<TItem, bool>> identifierFilter,
        string collectionName)
        where T : MongoEntityBase
    {
        var documentFilter = Builders<T>.Filter.Eq(x => x.ID, id);
        var itemFilter = Builders<TItem>.Filter.Where(identifierFilter);
        var combinedFilter = Builders<T>.Filter.And(documentFilter, Builders<T>.Filter.ElemMatch(arraySelector, itemFilter));

        var arrayFieldName = arraySelector.GetMemberName();
        var itemFieldName = itemSelector.GetMemberName();
        var fieldName = $"{arrayFieldName}.{itemFieldName}";
        var projection = Builders<T>.Projection.Exclude("_id").Include(fieldName);

        var result = await _database.GetCollection<T>(collectionName)
                                    .Find(combinedFilter)
                                    .Project(projection)
                                    .FirstOrDefaultAsync();

        var arrayFieldResult = this.GetFieldValueFromBsonDocument<List<TItem>>(result, arrayFieldName);
        return arrayFieldResult != null && arrayFieldResult.Count > 0
               ? itemSelector.Compile()(arrayFieldResult[0])
               : default;
    }

    public async Task<List<TResult>> GetFieldValuesInArrayAsync<T, TItem, TResult>(
        string id,
        Expression<Func<T, IEnumerable<TItem>>> arraySelector,
        Expression<Func<TItem, TResult>> itemSelector,
        string collectionName)
        where T : MongoEntityBase
    {
        var filter = Builders<T>.Filter.Eq(x => x.ID, id);

        var arrayFieldName = arraySelector.GetMemberName();
        var itemFieldName = itemSelector.GetMemberName();
        var fieldName = $"{arrayFieldName}.{itemFieldName}";
        var projection = Builders<T>.Projection.Exclude("_id").Include(fieldName);

        var result = await _database.GetCollection<T>(collectionName)
                                    .Find(filter)
                                    .Project(projection)
                                    .FirstOrDefaultAsync();

        var arrayFieldResult = this.GetFieldValueFromBsonDocument<List<TItem>>(result, arrayFieldName);
        return arrayFieldResult?.Select(itemSelector.Compile()).ToList() ?? [];
    }

    public async Task<MongoUpdateResult> UpdateFieldInArrayWithConditionAsync<T, TItem, TValue>(
        string id,
        Expression<Func<T, IEnumerable<TItem>>> arraySelector,
        Expression<Func<TItem, TValue>> fieldSelector,
        TValue value,
        Expression<Func<TItem, bool>> identifierFilter,
        string collectionName)
        where T : MongoEntityBase
        where TItem : class
    {
        var documentFilter = Builders<T>.Filter.Eq(x => x.ID, id);
        var itemFilter = Builders<TItem>.Filter.Where(identifierFilter);
        var combinedFilter = Builders<T>.Filter.And(documentFilter, Builders<T>.Filter.ElemMatch(arraySelector, itemFilter));

        var fieldName = $"{arraySelector.GetMemberName()}.$.{fieldSelector.GetMemberName()}";
        var update = Builders<T>.Update.Set(fieldName, value);

        var result = await _database.GetCollection<T>(collectionName)
                                    .UpdateOneAsync(combinedFilter, update);

        return new MongoUpdateResult(result);
    }

    public async Task<List<T>> GetPageQueryDataAsync<T>(PageQueryDTO queryDTO, string collectionName)
    {
        var collection = _database.GetCollection<T>(collectionName);
        var filter = Builders<T>.Filter.Empty;

        if (!string.IsNullOrEmpty(queryDTO.Search) && !string.IsNullOrEmpty(queryDTO.SearchBy))
        {
            var regex = new BsonRegularExpression(queryDTO.Search, "i");
            filter = Builders<T>.Filter.Regex(queryDTO.SearchBy, regex);
        }

        var query = collection.Find(filter);

        if (!string.IsNullOrEmpty(queryDTO.SortBy) && queryDTO.SortOrder != SortOrder.None)
        {
            var sortDefinition = queryDTO.SortOrder == SortOrder.Asc
                ? Builders<T>.Sort.Ascending(queryDTO.SortBy)
                : Builders<T>.Sort.Descending(queryDTO.SortBy);

            query = query.Sort(sortDefinition);
        }

        var result = await query.Skip(queryDTO.Skip).Limit(queryDTO.Top).ToListAsync();
        return result;
    }

    private TResult? GetFieldValueFromBsonDocument<TResult>(BsonDocument? document, string fieldName)
    {
        if (document != null && document.Contains(fieldName))
        {
            var bsonValue = document[fieldName];

            if (bsonValue.IsBsonArray)
            {
                // If the field is an array, deserialize it to the specified TResult
                var bsonArray = bsonValue.AsBsonArray;
                return BsonSerializer.Deserialize<TResult>(bsonArray.ToJson());
            }
            else if (bsonValue.IsBsonDocument)
            {
                // If it's a document, deserialize it as TResult
                var bsonDocument = bsonValue.AsBsonDocument;
                return BsonSerializer.Deserialize<TResult>(bsonDocument);
            }
            else
            {
                // For primitive types (like string, int, etc.), just cast it to TResult
                return (TResult)Convert.ChangeType(bsonValue, typeof(TResult));
            }
        }

        return default;
    }
}
