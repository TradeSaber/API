﻿using Microsoft.AspNetCore.Http;

namespace TradeSaber.Models
{
    public class UploadCard : ICard
    {
        public string Series { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Master { get; set; }
        public int MaxPrints { get; set; }
        public Rarity Rarity { get; set; }
        public bool Locked { get; set; }
        public double BaseProbability { get; set; }
        public IFormFile Cover { get; set; }
    }
}
