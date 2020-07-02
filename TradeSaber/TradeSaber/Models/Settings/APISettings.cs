namespace TradeSaber.Models.Settings
{
    public class APISettings : IAPISettings
    {
        public string HTI { get; set; }
    }

    public interface IAPISettings
    {
        string HTI { get; set; }
    }
}