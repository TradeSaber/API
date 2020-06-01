using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TradeSaber.Models
{
    public class Card
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string Series { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Master { get; set; }
        public int MaxPrints { get; set; } = -1;
        public Rarity Rarity { get; set; } = Rarity.Common;
        public bool Locked { get; set; } = false;
        public double BaseProbability { get; set; } = .05f;
        public string CoverURL { get; set; }
    }
}