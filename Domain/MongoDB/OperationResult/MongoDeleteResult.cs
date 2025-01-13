using MongoDB.Driver;

namespace MailCrafter.Domain;
public class MongoDeleteResult : MongoOperationResult
{
    public long DeletedCount { get; private set; }
    public bool DocumentFound => DeletedCount > 0;

    public MongoDeleteResult(DeleteResult result)
        : base(result.IsAcknowledged, result.IsAcknowledged && result.DeletedCount > 0)
    {
        DeletedCount = result.DeletedCount;
    }
}


