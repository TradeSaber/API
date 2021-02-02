using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TradeSaber.Models
{
    public class Series
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        [JsonIgnore]
        public Media Icon { get; set; } = null!;

        [NotMapped]
        public string IconURL => Icon.Path;


        [JsonIgnore]
        public Media Banner { get; set; } = null!;

        [NotMapped]
        public string BannerURL => Banner.Path;


        [Column(TypeName = "jsonb")]
        public ColorTheme Theme { get; set; } = null!;

        [JsonIgnore]
        public IList<Card> Cards { get; set; } = new List<Card>();


        public class Reference
        {
            public Guid ID { get; set; }
            public float? Boost { get; set; }

            [JsonIgnore]
            public Series Series { get; set; } = null!;

            [JsonPropertyName("series"), NotMapped]
            public Guid SeriesID => Series.ID;
        }
    }
}