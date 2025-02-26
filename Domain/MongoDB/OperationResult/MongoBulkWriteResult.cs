using MongoDB.Driver;

namespace MailCrafter.Domain;
public class MongoBulkWriteResult : MongoOperationResult
{
    public long MatchedCount { get; private set; }
    public long ModifiedCount { get; private set; }
    public long DeletedCount { get; private set; }
    public long InsertedCount { get; private set; }
    public bool HasErrors { get; private set; }

    public MongoBulkWriteResult(BulkWriteResult result)
        : base(result.IsAcknowledged, result.IsAcknowledged && result.ModifiedCount > 0)
    {
        MatchedCount = result.MatchedCount;
        ModifiedCount = result.ModifiedCount;
        DeletedCount = result.DeletedCount;
        InsertedCount = result.InsertedCount;
        HasErrors = result.RequestCount > (MatchedCount + ModifiedCount + DeletedCount + InsertedCount);
    }

    public MongoBulkWriteResult(bool isAcknowledged, bool isSuccessful)
        : base(isAcknowledged, isSuccessful)
    {

    }
}
