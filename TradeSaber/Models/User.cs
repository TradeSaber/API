using System;
using TradeSaber.Models.Discord;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeSaber.Models
{
    public class User
    {
        public Guid ID { get; set; }

        [Column(TypeName = "jsonb")]
        public DiscordUser Profile { get; set; } = null!;
    }
}