using System;
using System.Text.Json.Serialization;

namespace TradeSaber.Models
{
    public class Rarity
    {
        [JsonIgnore]
        public Guid ID { get; set; }

        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public float Probability { get; set; }
    }
}