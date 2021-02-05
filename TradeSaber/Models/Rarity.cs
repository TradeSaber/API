using System;
using System.Collections.Generic;
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

        [JsonIgnore]
        public IList<Pack.Reference> Packs { get; set; } = new List<Pack.Reference>();

        public class Reference
        {
            public Guid ID { get; set; }
            public Rarity Rarity { get; set; } = null!;
        }
    }
}