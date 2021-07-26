namespace TradeSaber
{
    public static class Scopes
    {
        public const string GameSession = "game:session";

        public static readonly string[] AllScopes = new string[]
        {
            GameSession
        };
    }
}