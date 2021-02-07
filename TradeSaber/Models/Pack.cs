using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace TradeSaber.Models
{
    public class Pack
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        

        [JsonIgnore]
        public Media Cover { get; set; } = null!;

        [NotMapped]
        public string CoverURL => Cover.Path;
    
        public float? Value { get; set; }
        public int CardCount { get; set; }


        [Column(TypeName = "jsonb")]
        public ColorTheme Theme { get; set; } = null!;

        [JsonIgnore]
        public IList<Rarity.Reference> Rarities { get; set; } = new List<Rarity.Reference>();

        public IList<Card.Reference> CardPool { get; set; } = new List<Card.Reference>();
        
        [JsonIgnore]
        public IList<Card> Cards { get; set; } = new List<Card>();

        [JsonPropertyName("rarities"), NotMapped]
        public IEnumerable<Rarity> ReferencedRarities => Rarities.Select(r => r.Rarity);

        public class Reference
        {
            public Guid ID { get; set; }

            [JsonIgnore]
            public Pack Pack { get; set; } = null!;

            [JsonPropertyName("pack")]
            public Guid PackID => Pack.ID;
        }

        public class TradeableReference
        {
            public Guid ID { get; set; }

            [JsonIgnore]
            public Pack Pack { get; set; } = null!;

            [JsonPropertyName("pack")]
            public Guid PackID => Pack.ID;
        }

        public class TradeableRReference
        {
            public Guid ID { get; set; }

            [JsonIgnore]
            public Pack Pack { get; set; } = null!;

            [JsonPropertyName("pack")]
            public Guid PackID => Pack.ID;
        }
    }
}