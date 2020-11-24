using System;

namespace TradeSaber.Models
{
    public class Card : IEquatable<Card>
    {
        public Guid ID { get; set; }
        public Series Series { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public float Probability { get; set; }
        public Rarity Rarity { get; set; }
        public string? Root { get; set; }
        public int? Maximum { get; set; }
        public bool Locked { get; set; }
        public string CoverURL { get; set; } = null!;
        public string BaseURL { get; set; } = null!;
        public float? Value { get; set; }

        #region Equatable

        public bool Equals(Card? other)
        {
            return ID == other?.ID;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Card);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public static bool operator !=(Card c1, Card c2)
        {
            return !(c1 == c2);
        }

        public static bool operator ==(Card c1, Card c2)
        {
            return c1 == c2 || c1.Equals(c2);
        }

        #endregion
    }
}