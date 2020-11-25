using System;
using NodaTime;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeSaber.Models
{
    public class Session
    {
        public Guid ID { get; set; }
        public Instant StartTime { get; set; }
        public User User { get; set; } = null!;

        [Column(TypeName = "jsonb")]
        public Dictionary<Level, Score> Scores { get; set; } = new Dictionary<Level, Score>();

        public record Level(string Hash, Difficulty Difficulty, string Characteristic);

        public struct Score
        {
            public int RawScore { get; set; }
            public int ModScore { get; set; }
            public Instant Uploaded { get; set; }

            public override bool Equals(object? obj)
            {
                return obj is Score score && ModScore == score.ModScore;
            }

            public override int GetHashCode()
            {
                return RawScore ^ ModScore;
            }

            public override string? ToString()
            {
                return $"Raw: {RawScore}, Mod: {ModScore}";
            }

            public static bool operator ==(Score left, Score right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Score left, Score right)
            {
                return !(left == right);
            }

            public static bool operator >=(Score left, Score right)
            {
                return left.ModScore >= right.ModScore;
            }

            public static bool operator <=(Score left, Score right)
            {
                return left.ModScore <= right.ModScore;
            }

            public static bool operator >(Score left, Score right)
            {
                return left.ModScore > right.ModScore;
            }

            public static bool operator <(Score left, Score right)
            {
                return left.ModScore < right.ModScore;
            }
        }
    }
}
