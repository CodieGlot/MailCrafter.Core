namespace MailCrafter.Domain;
public class MongoInsertResult : MongoOperationResult
{
    public string InsertedID { get; private set; }
    public MongoInsertResult(bool isAcknowledged, string insertedId)
        : base(isAcknowledged, isAcknowledged)
    {
        InsertedID = isAcknowledged ? insertedId : string.Empty;
    }
}

