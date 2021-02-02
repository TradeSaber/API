using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradeSaber.Models
{
    public class Inventory
    {
        public Guid ID { get; set; }
        public float TirCoin { get; set; }
        public IList<Card> Cards { get; set; } = new List<Card>();
        public IList<Pack> Packs { get; set; } = new List<Pack>();
    }
}