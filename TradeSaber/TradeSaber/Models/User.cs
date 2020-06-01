using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace TradeSaber.Models
{
    public class User
    {
        [BsonId]
        [JsonIgnore]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public bool Banned { get; set; }
        public string DiscordID { get; set; }
        public TradeSaberRole Role { get; set; }
        public DiscordUser Profile { get; set; }
        public List<string> Inventory { get; set; }
        public List<string> UnopenedPacks { get; set; }
    }
}