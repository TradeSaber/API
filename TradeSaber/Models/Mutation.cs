using System;
using System.Collections.Generic;

namespace TradeSaber.Models
{
    public class Mutation
    {
        public Guid ID { get; set; }

        public bool Active { get; set; }
        public string Name { get; set; } = null!;
        public float? GlobalXPBoost { get; set; }
        public float? GlobalTirBoost { get; set; }
        public IList<Card.Reference> Cards { get; set; } = new List<Card.Reference>();
        public IList<Series.Reference> Series { get; set; } = new List<Series.Reference>();
    }
}