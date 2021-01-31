using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
    
        
        public int CardCount { get; set; }
        public ColorTheme Theme { get; set; } = null!;


        [JsonIgnore]
        public IList<Card.Reference> CardPool { get; set; } = new List<Card.Reference>();
        
        [JsonIgnore]
        public IList<Rarity> Rarities { get; set; } = new List<Rarity>();
        
        [JsonIgnore]
        public IList<Card> Cards { get; set; } = new List<Card>();
    }
}