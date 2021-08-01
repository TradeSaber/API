using Microsoft.EntityFrameworkCore;
using TradeSaber.Models;

namespace TradeSaber
{
    public class TradeContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();

        public TradeContext(DbContextOptions<TradeContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo((id, level) => false, @event => _ = @event);
        }
    }
}