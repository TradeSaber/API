using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TradeSaber.Models
{
    public class Objective
    {
        public Guid ID { get; set; }
        public bool Active { get; set; }
        public bool Special { get; set; }

        [JsonIgnore]
        public Media Icon { get; set; } = null!;

        [NotMapped]
        public string? IconURL => Icon?.Path;

        [JsonPropertyName("type")]
        public Type ObjectiveType { get; set; }
        public float? XPReward { get; set; }
        public float? TirReward { get; set; } 
        public string? Subject { get; set; } = null!;
        public string Template { get; set; } = null!;
        
        [JsonIgnore]
        public IList<Result> ObjectiveResults { get; set; } = new List<Result>();
        public IList<Pack.Reference> PackRewards { get; set; } = new List<Pack.Reference>();

        public enum Type
        {
            Unknown,
            PlayLevel,
            PlayXLevels,
            UseModifier,
            SessionLength,
            WinMultiplayerMatch,
            PlayMultiplayerMatch,
        }

        public class Result
        {
            public Guid ID { get; set; }
            public User Submitter { get; set; } = null!;
            public Objective Objective { get; set; } = null!;
            public DateTime Submitted { get; set; }
            public float? Modifier { get; set; }
        }

        public class Data
        {
            [JsonPropertyName("objectiveRange")]
            public int[] ObjectiveRange { get; set; } = null!;

            [JsonPropertyName("playRange")]
            public int[] PlayRange { get; set; } = null!;

            [JsonPropertyName("maxPercents")]
            public float[] MaxPercents { get; set; } = null!;

            [JsonPropertyName("modifiers")]
            public Modifier[] Modifiers { get; set; } = null!;

            [JsonPropertyName("sessionLengths")]
            public float[] SessionLengths { get; set; } = null!;

            [JsonPropertyName("tirRewards")]
            public float[] TirRewards { get; set; } = null!;

            [JsonPropertyName("xpRewards")]
            public float[] XPRewards { get; set; } = null!;

            [JsonPropertyName("packRewards")]
            public Packet[] PackRewards { get; set; } = null!;

            [JsonPropertyName("levels")]
            public Level[] Levels { get; set; } = null!;

            public class Level
            {
                [JsonPropertyName("key")]
                public string? Key { get; set; }

                [JsonPropertyName("hash")]
                public string Hash { get; set; } = null!;

                [JsonPropertyName("difficulty")]
                public Difficulty? LevelDifficulty { get; set; }

                [JsonPropertyName("characteristic")]
                public string? Characteristic { get; set; }

                public enum Difficulty
                {
                    Easy,
                    Normal,
                    Hard,
                    Expert,
                    ExpertPlus
                }
            }

            [Flags]
            public enum Modifier : ushort
            {
                None = 0,
                BatteryEnergy = 1,
                NoFail = 2,
                InstaFail = 4,
                NoObstacles = 8,
                NoBombs = 16,
                FastNotes = 32,
                StrictAngles = 64,
                DisappearingArrows = 128,
                FasterSong = 256,
                SlowerSong = 512,
                NoArrows = 1024,
                GhostNotes = 2048,
                All = 4095
            }

            public class Packet
            {
                public int Count { get; set; }
                public Guid Pack { get; set; }
            }
        }
    }
}