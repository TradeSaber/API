using TradeSaber.Models;
using Microsoft.EntityFrameworkCore;

namespace TradeSaber
{
    public class TradeContext : DbContext
    {
        public DbSet<Card> Cards { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Series> Series { get; set; } = null!;

        public TradeContext(DbContextOptions<TradeContext> options) : base(options)
        {

        }
    }
}