using System;
using System.Text.Json.Serialization;

namespace TradeSaber.Models
{
    public class Media
    {
        public Guid ID { get; set; }
        public long FileSize { get; set; }
        public string Path { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string FileHash { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public User Uploader { get; set; } = null!;
    }
}