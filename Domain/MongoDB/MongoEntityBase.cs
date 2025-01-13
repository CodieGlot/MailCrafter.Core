using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MailCrafter.Domain;
public class MongoEntityBase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ID { get; set; }
}
