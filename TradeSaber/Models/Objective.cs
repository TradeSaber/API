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

        [JsonIgnore]
        public Media Icon { get; set; } = null!;

        [NotMapped]
        public string IconURL => Icon.Path;

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
    }
}