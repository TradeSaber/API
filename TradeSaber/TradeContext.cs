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
        public DbSet<Mutation> Mutations => Set<Mutation>();
        public DbSet<Objective> Objectives => Set<Objective>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        
        public TradeContext(DbContextOptions<TradeContext> options) : base (options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo((string _) => { });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card.Reference>().ToTable("card_references");
            modelBuilder.Entity<Pack.Reference>().ToTable("pack_references");
            modelBuilder.Entity<Series.Reference>().ToTable("series_references");
            modelBuilder.Entity<Rarity.Reference>().ToTable("rarity_references");
            modelBuilder.Entity<Objective.Result>().ToTable("objective_results");
            modelBuilder.Entity<Card.TradeableReference>().ToTable("tradeable_cards");
            modelBuilder.Entity<Pack.TradeableReference>().ToTable("tradeable_packs");
            modelBuilder.Entity<Card.TradeableRReference>().ToTable("tradeable_r_cards");
            modelBuilder.Entity<Pack.TradeableRReference>().ToTable("tradeable_r_packs");
        }
    }
}