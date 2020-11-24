namespace TradeSaber.Settings
{
    public class RaritySettings
    {
        public Pair Common { get; init; } = new Pair("white", 0.6f);
        public Pair Uncommon { get; init; } = new Pair("#26f1ff", 0.3f);
        public Pair Rare { get; init; } = new Pair("#d426ff", 0.09f);
        public Pair Legendary { get; init; } = new Pair("goldenrod", 0.01f);

        public record Pair(string Color, float Probability);
    }
}