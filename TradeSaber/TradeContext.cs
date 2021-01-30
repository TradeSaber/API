using Microsoft.EntityFrameworkCore;
using TradeSaber.Models;

namespace TradeSaber
{
    public class TradeContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public TradeContext(DbContextOptions<TradeContext> options) : base (options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo((string _) => { });
        }
    }
}