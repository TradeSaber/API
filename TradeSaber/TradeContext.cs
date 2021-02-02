using Microsoft.EntityFrameworkCore;
using TradeSaber.Models;

namespace TradeSaber
{
    public class TradeContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Card> Cards => Set<Card>();
        public DbSet<Pack> Packs => Set<Pack>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Media> Media => Set<Media>();
        public DbSet<Series> Series => Set<Series>();
        public DbSet<Rarity> Rarities => Set<Rarity>();
        public DbSet<Inventory> Inventories => Set<Inventory>();

        public TradeContext(DbContextOptions<TradeContext> options) : base (options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo((string _) => { });
        }
    }
}