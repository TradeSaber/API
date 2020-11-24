using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeSaber.Models
{
    public class Pack
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CoverURL { get; set; } = null!;
        public string Theme { get; set; } = null!;
        public float? Value { get; set; }
        public int Count { get; set; }

        public IList<Card.Reference> CardPool { get; set; } = null!;

        [Column(TypeName = "jsonb")]
        public IList<Rarity> Rarities { get; set; } = null!;
        public IList<Card> Cards { get; set; } = null!;
        public IList<User> OwnedBy { get; set; } = null!;
    }
}