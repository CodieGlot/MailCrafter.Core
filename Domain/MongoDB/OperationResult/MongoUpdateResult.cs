using MongoDB.Driver;

namespace MailCrafter.Domain;
public class MongoUpdateResult : MongoOperationResult
{
    public long MatchedCount { get; private set; }
    public long ModifiedCount { get; private set; }
    public bool DocumentFound => MatchedCount > 0;

    public MongoUpdateResult(UpdateResult result)
        : base(result.IsAcknowledged, result.IsAcknowledged && result.ModifiedCount > 0)
    {
        MatchedCount = result.MatchedCount;
        ModifiedCount = result.ModifiedCount;
    }
}


