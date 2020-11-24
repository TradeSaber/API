using TradeSaber.Models;
using Microsoft.EntityFrameworkCore;

namespace TradeSaber
{
    public class TradeContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public TradeContext(DbContextOptions<TradeContext> options) : base(options)
        {

        }
    }
}