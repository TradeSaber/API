using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TradeSaber.Models
{
    public class Role
    {
        [JsonIgnore]
        public Guid ID { get; set; }
        public string Name { get; set; } = null!;
        public List<string> Scopes { get; set; } = new();

        [JsonIgnore]
        public bool Root { get; set; }
    }
}