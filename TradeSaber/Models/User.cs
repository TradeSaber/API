using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TradeSaber.Models.Discord;

namespace TradeSaber.Models
{
    public class User
    {
        public Guid ID { get; set; }

        [Column(TypeName = "jsonb")]
        public DiscordUser Profile { get; set; } = null!;

        public Role? Role { get; set; }

        public float XP { get; set; }

        [JsonIgnore]
        public Inventory Inventory { get; set; } = null!;

        [Column(TypeName = "jsonb")]
        public Settings Settings { get; set; } = new Settings();

        [JsonIgnore]
        public IList<Objective.Result> CompletedObjectives { get; set; } = new List<Objective.Result>();
    }
}