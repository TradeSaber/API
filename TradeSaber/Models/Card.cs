using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TradeSaber.Models
{
    public class Card
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;


        public Rarity Rarity { get; set; } = null!;
        public Series Series { get; set; } = null!;


        public bool Public { get; set; }
        public int? Maximum { get; set; }
        public float? Value { get; set; }
        public float Probability { get; set; }


        [JsonIgnore]
        public Media Base { get; set; } = null!;

        [NotMapped]
        public string BaseURL => Base.Path;


        [JsonIgnore]
        public Media Cover { get; set; } = null!;

        [NotMapped]
        public string CoverURL => Cover.Path;


        [JsonIgnore]
        public IList<Pack> Packs { get; set; } = new List<Pack>();

        [JsonIgnore]
        public IList<Inventory> OwnedBy { get; set; } = new List<Inventory>();

        public class Reference
        {
            public Guid ID { get; set; }
            public bool Guaranteed { get; set; }
            public Card Card { get; set; } = null!;
            public float? Boost { get; set; } = null;
        }
    }
}