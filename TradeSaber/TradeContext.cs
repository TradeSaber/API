using TradeSaber.Models;
using Microsoft.EntityFrameworkCore;

namespace TradeSaber
{
    public class TradeContext : DbContext
    {
        public DbSet<Card> Cards { get; set; } = null!;
        public DbSet<Pack> Packs { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Series> Series { get; set; } = null!;

        public TradeContext(DbContextOptions<TradeContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Card.Reference>().HasOne(cr => cr.Card);
            modelBuilder.Entity<Card.Reference>().HasKey(cr => cr.ID);
            modelBuilder.Entity<Pack>().HasMany(cr => cr.CardPool);
        }
    }
}