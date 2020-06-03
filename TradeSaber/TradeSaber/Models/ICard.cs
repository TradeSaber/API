namespace TradeSaber.Models
{
    public interface ICard
    {
        string Series { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string Master { get; set; }
        int MaxPrints { get; set; }
        Rarity Rarity { get; set; }
        bool Locked { get; set; }
        double BaseProbability { get; set; }
    }
}
