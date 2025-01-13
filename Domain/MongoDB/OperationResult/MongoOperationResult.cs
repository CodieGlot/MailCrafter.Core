namespace MailCrafter.Domain;
public abstract class MongoOperationResult
{
    public bool IsAcknowledged { get; protected set; }
    public bool IsSuccessful { get; protected set; }

    protected MongoOperationResult(bool isAcknowledged, bool isSuccessful)
    {
        IsAcknowledged = isAcknowledged;
        IsSuccessful = isSuccessful;
    }
}


