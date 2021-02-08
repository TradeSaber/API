using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace TradeSaber.Models
{
    public class Inventory
    {
        public Guid ID { get; set; }
        public float TirCoin { get; set; }
        public IList<Pack.Reference> Packs { get; set; } = new List<Pack.Reference>();
        public IList<Card.TradeableReference> Cards { get; set; } = new List<Card.TradeableReference>();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotMapped]
        public float? PortfolioValue => Cards?.Sum(c => c.Card.Value.GetValueOrDefault());
    
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotMapped]
        public int? Rank { get; set; }
    }
}