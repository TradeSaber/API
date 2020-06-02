using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TradeSaber.Models
{
    public class Series
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
