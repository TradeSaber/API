using System.Collections.Generic;

namespace TradeSaber.Models.Settings
{
    public interface IPack
    {
        string Name { get; set; }
        string Description { get; set; }
        int Count { get; set; }
        List<ProbabilityDatum> LockedCardPool { get; set; }
        List<string> GuaranteedCards { get; set; }
        List<Rarity> GuaranteedRarities { get; set; }
        string Theme { get; set; }
    }
}
