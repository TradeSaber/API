using System;
using System.Collections.Generic;

namespace TradeSaber.Models
{
    public class Inventory
    {
        public Guid ID { get; set; }
        public float TirCoin { get; set; }
        public IList<Card.Reference> Cards { get; set; } = new List<Card.Reference>();
        public IList<Pack.Reference> Packs { get; set; } = new List<Pack.Reference>();
    }
}