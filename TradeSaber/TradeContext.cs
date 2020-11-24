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
        public DbSet<Mutation> Mutations { get; set; } = null!;

        public TradeContext(DbContextOptions<TradeContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Card.Reference>().ToTable("card_reference");
            modelBuilder.Entity<Card.Reference>().HasOne(cr => cr.Card);
            modelBuilder.Entity<Card.Reference>().HasKey(cr => cr.ID).HasName("pk_card_reference");

            modelBuilder.Entity<Series.Reference>().ToTable("series_reference");
            modelBuilder.Entity<Series.Reference>().HasOne(sr => sr.Series);
            modelBuilder.Entity<Series.Reference>().HasKey(sr => sr.ID).HasName("pk_series_reference");

            modelBuilder.Entity<Pack>().HasMany(p => p.CardPool);
            modelBuilder.Entity<Mutation>().HasMany(mu => mu.Series);
            modelBuilder.Entity<Mutation>().HasMany(mu => mu.Cards);
        }
    }
}