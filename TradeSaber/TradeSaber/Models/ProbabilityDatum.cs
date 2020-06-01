using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TradeSaber.Models
{
    public class ProbabilityDatum
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public double ProbabilityBoost { get; set; }
    }
}
