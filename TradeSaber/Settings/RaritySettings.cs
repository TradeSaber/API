namespace TradeSaber.Settings
{
    public class RaritySettings
    {
        public Pair Common { get; init; }
        public Pair Uncommon { get; init; }
        public Pair Rare { get; init; }
        public Pair Legendary { get; init; }

        public record Pair(string Color, float Probability);
    }
}