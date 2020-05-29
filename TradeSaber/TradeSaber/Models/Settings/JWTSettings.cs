namespace TradeSaber.Models.Settings
{
    public class JWTSettings : IJWTSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
    }

    public interface IJWTSettings
    {
        string Key { get; set; }
        string Issuer { get; set; }
    }
}
