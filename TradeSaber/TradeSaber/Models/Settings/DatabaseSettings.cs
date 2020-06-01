namespace TradeSaber.Models.Settings
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string UserCollection { get; set; }
        public string CardCollection { get; set; }
        public string PackCollection { get; set; }
        public string SeriesCollection { get; set; }
    }

    public interface IDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string UserCollection { get; set; }
        string CardCollection { get; set; }
        string PackCollection { get; set; }
        string SeriesCollection { get; set; }
    }
}