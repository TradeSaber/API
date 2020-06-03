using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeSaber.Models.Settings;

namespace TradeSaber.Models
{
    public class UploadPack : IPack
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public List<ProbabilityDatum> LockedCardPool { get; set; }
        public List<string> GuaranteedCards { get; set; }
        public List<Rarity> GuaranteedRarities { get; set; }
        public string Theme { get; set; }
        public IFormFile Cover { get; set; }
        public string ProbData { get; set; }
        public string CardData { get; set; }
        public string RarityData { get; set; }
    }
}
