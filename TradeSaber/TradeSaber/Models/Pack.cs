using MongoDB.Bson;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace TradeSaber.Models
{
    public class Pack
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; } = 5;
        public List<ProbabilityDatum> LockedCardPool { get; set; } = new List<ProbabilityDatum>();
        public List<string> GuaranteedCards { get; set; } = new List<string>();
        public List<Rarity> GuaranteedRarities { get; set; } = new List<Rarity>();
        public string Theme { get; set; } = "#ffffff";
        public string CoverURL { get; set; } 
    }
}