using System;
using System.Collections.Generic;

namespace TradeSaber.Models
{
    public class Series
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string MainColor { get; set; } = null!;
        public string SubColor { get; set; } = null!;
        public string IconURL { get; set; } = null!;
        public string BannerURL { get; set; } = null!;

        public IList<Card> Cards { get; set; } = null!;

        public class Reference
        {
            public Guid ID { get; set; }
            public Series Series { get; set; } = null!;
            public float? Boost { get; set; }
        }
    }
}