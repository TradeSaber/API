namespace TradeSaber.Models
{
    public class Settings
    {
        public bool AcceptTrades { get; set; } = true;
        public InventoryPrivacy Privacy { get; set; }

        public enum InventoryPrivacy
        {
            Public,
            Private
        }
    }
}