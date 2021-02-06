﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TradeSaber;
using TradeSaber.Models;
using TradeSaber.Models.Discord;

namespace TradeSaber.Migrations
{
    [DbContext(typeof(TradeContext))]
    partial class TradeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("CardPack", b =>
                {
                    b.Property<Guid>("CardsID")
                        .HasColumnType("uuid")
                        .HasColumnName("cards_id");

                    b.Property<Guid>("PacksID")
                        .HasColumnType("uuid")
                        .HasColumnName("packs_id");

                    b.HasKey("CardsID", "PacksID")
                        .HasName("pk_card_pack");

                    b.HasIndex("PacksID")
                        .HasDatabaseName("ix_card_pack_packs_id");

                    b.ToTable("card_pack");
                });

            modelBuilder.Entity("TradeSaber.Models.Card", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("BaseID")
                        .HasColumnType("uuid")
                        .HasColumnName("base_id");

                    b.Property<Guid?>("CoverID")
                        .HasColumnType("uuid")
                        .HasColumnName("cover_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<int?>("Maximum")
                        .HasColumnType("integer")
                        .HasColumnName("maximum");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<float>("Probability")
                        .HasColumnType("real")
                        .HasColumnName("probability");

                    b.Property<bool>("Public")
                        .HasColumnType("boolean")
                        .HasColumnName("public");

                    b.Property<Guid?>("RarityID")
                        .HasColumnType("uuid")
                        .HasColumnName("rarity_id");

                    b.Property<Guid>("SeriesID")
                        .HasColumnType("uuid")
                        .HasColumnName("series_id");

                    b.Property<float?>("Value")
                        .HasColumnType("real")
                        .HasColumnName("value");

                    b.HasKey("ID")
                        .HasName("pk_cards");

                    b.HasIndex("BaseID")
                        .HasDatabaseName("ix_cards_base_id");

                    b.HasIndex("CoverID")
                        .HasDatabaseName("ix_cards_cover_id");

                    b.HasIndex("RarityID")
                        .HasDatabaseName("ix_cards_rarity_id");

                    b.HasIndex("SeriesID")
                        .HasDatabaseName("ix_cards_series_id");

                    b.ToTable("cards");
                });

            modelBuilder.Entity("TradeSaber.Models.Card+Reference", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<float?>("Boost")
                        .HasColumnType("real")
                        .HasColumnName("boost");

                    b.Property<Guid?>("CardID1")
                        .HasColumnType("uuid")
                        .HasColumnName("card_id1");

                    b.Property<bool>("Guaranteed")
                        .HasColumnType("boolean")
                        .HasColumnName("guaranteed");

                    b.Property<Guid?>("InventoryID")
                        .HasColumnType("uuid")
                        .HasColumnName("inventory_id");

                    b.Property<Guid?>("MutationID")
                        .HasColumnType("uuid")
                        .HasColumnName("mutation_id");

                    b.Property<Guid?>("PackID")
                        .HasColumnType("uuid")
                        .HasColumnName("pack_id");

                    b.HasKey("ID")
                        .HasName("pk_card_references");

                    b.HasIndex("CardID1")
                        .HasDatabaseName("ix_card_references_card_id1");

                    b.HasIndex("InventoryID")
                        .HasDatabaseName("ix_card_references_inventory_id");

                    b.HasIndex("MutationID")
                        .HasDatabaseName("ix_card_references_mutation_id");

                    b.HasIndex("PackID")
                        .HasDatabaseName("ix_card_references_pack_id");

                    b.ToTable("card_references");
                });

            modelBuilder.Entity("TradeSaber.Models.Inventory", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("CardID")
                        .HasColumnType("uuid")
                        .HasColumnName("card_id");

                    b.Property<float>("TirCoin")
                        .HasColumnType("real")
                        .HasColumnName("tir_coin");

                    b.HasKey("ID")
                        .HasName("pk_inventories");

                    b.HasIndex("CardID")
                        .HasDatabaseName("ix_inventories_card_id");

                    b.ToTable("inventories");
                });

            modelBuilder.Entity("TradeSaber.Models.Media", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("FileHash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("file_hash");

                    b.Property<long>("FileSize")
                        .HasColumnType("bigint")
                        .HasColumnName("file_size");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("mime_type");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("path");

                    b.Property<Guid?>("UploaderID")
                        .HasColumnType("uuid")
                        .HasColumnName("uploader_id");

                    b.HasKey("ID")
                        .HasName("pk_media");

                    b.HasIndex("UploaderID")
                        .HasDatabaseName("ix_media_uploader_id");

                    b.ToTable("media");
                });

            modelBuilder.Entity("TradeSaber.Models.Mutation", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean")
                        .HasColumnName("active");

                    b.Property<float?>("GlobalTirBoost")
                        .HasColumnType("real")
                        .HasColumnName("global_tir_boost");

                    b.Property<float?>("GlobalXPBoost")
                        .HasColumnType("real")
                        .HasColumnName("global_xp_boost");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("ID")
                        .HasName("pk_mutations");

                    b.ToTable("mutations");
                });

            modelBuilder.Entity("TradeSaber.Models.Objective", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean")
                        .HasColumnName("active");

                    b.Property<Guid?>("IconID")
                        .HasColumnType("uuid")
                        .HasColumnName("icon_id");

                    b.Property<int>("ObjectiveType")
                        .HasColumnType("integer")
                        .HasColumnName("objective_type");

                    b.Property<string>("Subject")
                        .HasColumnType("text")
                        .HasColumnName("subject");

                    b.Property<string>("Template")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("template");

                    b.Property<float?>("TirReward")
                        .HasColumnType("real")
                        .HasColumnName("tir_reward");

                    b.Property<float?>("XPReward")
                        .HasColumnType("real")
                        .HasColumnName("xp_reward");

                    b.HasKey("ID")
                        .HasName("pk_objectives");

                    b.HasIndex("IconID")
                        .HasDatabaseName("ix_objectives_icon_id");

                    b.ToTable("objectives");
                });

            modelBuilder.Entity("TradeSaber.Models.Objective+Result", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<float?>("Modifier")
                        .HasColumnType("real")
                        .HasColumnName("modifier");

                    b.Property<Guid?>("ObjectiveID")
                        .HasColumnType("uuid")
                        .HasColumnName("objective_id");

                    b.Property<DateTime>("Submitted")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("submitted");

                    b.Property<Guid?>("SubmitterID")
                        .HasColumnType("uuid")
                        .HasColumnName("submitter_id");

                    b.HasKey("ID")
                        .HasName("pk_objective_results");

                    b.HasIndex("ObjectiveID")
                        .HasDatabaseName("ix_objective_results_objective_id");

                    b.HasIndex("SubmitterID")
                        .HasDatabaseName("ix_objective_results_submitter_id");

                    b.ToTable("objective_results");
                });

            modelBuilder.Entity("TradeSaber.Models.Pack", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("CardCount")
                        .HasColumnType("integer")
                        .HasColumnName("card_count");

                    b.Property<Guid?>("CoverID")
                        .HasColumnType("uuid")
                        .HasColumnName("cover_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<ColorTheme>("Theme")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("theme");

                    b.Property<float?>("Value")
                        .HasColumnType("real")
                        .HasColumnName("value");

                    b.HasKey("ID")
                        .HasName("pk_packs");

                    b.HasIndex("CoverID")
                        .HasDatabaseName("ix_packs_cover_id");

                    b.ToTable("packs");
                });

            modelBuilder.Entity("TradeSaber.Models.Pack+Reference", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("InventoryID")
                        .HasColumnType("uuid")
                        .HasColumnName("inventory_id");

                    b.Property<Guid?>("ObjectiveID")
                        .HasColumnType("uuid")
                        .HasColumnName("objective_id");

                    b.Property<Guid?>("PackID1")
                        .HasColumnType("uuid")
                        .HasColumnName("pack_id1");

                    b.Property<Guid?>("RarityID")
                        .HasColumnType("uuid")
                        .HasColumnName("rarity_id");

                    b.HasKey("ID")
                        .HasName("pk_pack_references");

                    b.HasIndex("InventoryID")
                        .HasDatabaseName("ix_pack_references_inventory_id");

                    b.HasIndex("ObjectiveID")
                        .HasDatabaseName("ix_pack_references_objective_id");

                    b.HasIndex("PackID1")
                        .HasDatabaseName("ix_pack_references_pack_id1");

                    b.HasIndex("RarityID")
                        .HasDatabaseName("ix_pack_references_rarity_id");

                    b.ToTable("pack_references");
                });

            modelBuilder.Entity("TradeSaber.Models.Rarity", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("color");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<float>("Probability")
                        .HasColumnType("real")
                        .HasColumnName("probability");

                    b.HasKey("ID")
                        .HasName("pk_rarities");

                    b.ToTable("rarities");
                });

            modelBuilder.Entity("TradeSaber.Models.Rarity+Reference", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("PackID")
                        .HasColumnType("uuid")
                        .HasColumnName("pack_id");

                    b.Property<Guid?>("RarityID")
                        .HasColumnType("uuid")
                        .HasColumnName("rarity_id");

                    b.HasKey("ID")
                        .HasName("pk_rarity_references");

                    b.HasIndex("PackID")
                        .HasDatabaseName("ix_rarity_references_pack_id");

                    b.HasIndex("RarityID")
                        .HasDatabaseName("ix_rarity_references_rarity_id");

                    b.ToTable("rarity_references");
                });

            modelBuilder.Entity("TradeSaber.Models.Role", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<List<string>>("Scopes")
                        .IsRequired()
                        .HasColumnType("text[]")
                        .HasColumnName("scopes");

                    b.HasKey("ID")
                        .HasName("pk_roles");

                    b.ToTable("roles");
                });

            modelBuilder.Entity("TradeSaber.Models.Series", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("BannerID")
                        .HasColumnType("uuid")
                        .HasColumnName("banner_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<Guid?>("IconID")
                        .HasColumnType("uuid")
                        .HasColumnName("icon_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<ColorTheme>("Theme")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("theme");

                    b.HasKey("ID")
                        .HasName("pk_series");

                    b.HasIndex("BannerID")
                        .HasDatabaseName("ix_series_banner_id");

                    b.HasIndex("IconID")
                        .HasDatabaseName("ix_series_icon_id");

                    b.ToTable("series");
                });

            modelBuilder.Entity("TradeSaber.Models.Series+Reference", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<float?>("Boost")
                        .HasColumnType("real")
                        .HasColumnName("boost");

                    b.Property<Guid?>("MutationID")
                        .HasColumnType("uuid")
                        .HasColumnName("mutation_id");

                    b.Property<Guid?>("SeriesID1")
                        .HasColumnType("uuid")
                        .HasColumnName("series_id1");

                    b.HasKey("ID")
                        .HasName("pk_series_references");

                    b.HasIndex("MutationID")
                        .HasDatabaseName("ix_series_references_mutation_id");

                    b.HasIndex("SeriesID1")
                        .HasDatabaseName("ix_series_references_series_id1");

                    b.ToTable("series_references");
                });

            modelBuilder.Entity("TradeSaber.Models.User", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("InventoryID")
                        .HasColumnType("uuid")
                        .HasColumnName("inventory_id");

                    b.Property<DiscordUser>("Profile")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("profile");

                    b.Property<Guid?>("RoleID")
                        .HasColumnType("uuid")
                        .HasColumnName("role_id");

                    b.Property<float>("XP")
                        .HasColumnType("real")
                        .HasColumnName("xp");

                    b.HasKey("ID")
                        .HasName("pk_users");

                    b.HasIndex("InventoryID")
                        .HasDatabaseName("ix_users_inventory_id");

                    b.HasIndex("RoleID")
                        .HasDatabaseName("ix_users_role_id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("CardPack", b =>
                {
                    b.HasOne("TradeSaber.Models.Card", null)
                        .WithMany()
                        .HasForeignKey("CardsID")
                        .HasConstraintName("fk_card_pack_cards_cards_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TradeSaber.Models.Pack", null)
                        .WithMany()
                        .HasForeignKey("PacksID")
                        .HasConstraintName("fk_card_pack_packs_packs_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TradeSaber.Models.Card", b =>
                {
                    b.HasOne("TradeSaber.Models.Media", "Base")
                        .WithMany()
                        .HasForeignKey("BaseID")
                        .HasConstraintName("fk_cards_media_base_id");

                    b.HasOne("TradeSaber.Models.Media", "Cover")
                        .WithMany()
                        .HasForeignKey("CoverID")
                        .HasConstraintName("fk_cards_media_cover_id");

                    b.HasOne("TradeSaber.Models.Rarity", "Rarity")
                        .WithMany()
                        .HasForeignKey("RarityID")
                        .HasConstraintName("fk_cards_rarities_rarity_id");

                    b.HasOne("TradeSaber.Models.Series", "Series")
                        .WithMany("Cards")
                        .HasForeignKey("SeriesID")
                        .HasConstraintName("fk_cards_series_series_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Base");

                    b.Navigation("Cover");

                    b.Navigation("Rarity");

                    b.Navigation("Series");
                });

            modelBuilder.Entity("TradeSaber.Models.Card+Reference", b =>
                {
                    b.HasOne("TradeSaber.Models.Card", "Card")
                        .WithMany()
                        .HasForeignKey("CardID1")
                        .HasConstraintName("fk_card_references_cards_card_id1");

                    b.HasOne("TradeSaber.Models.Inventory", null)
                        .WithMany("Cards")
                        .HasForeignKey("InventoryID")
                        .HasConstraintName("fk_card_references_inventories_inventory_id");

                    b.HasOne("TradeSaber.Models.Mutation", null)
                        .WithMany("Cards")
                        .HasForeignKey("MutationID")
                        .HasConstraintName("fk_card_references_mutations_mutation_id");

                    b.HasOne("TradeSaber.Models.Pack", null)
                        .WithMany("CardPool")
                        .HasForeignKey("PackID")
                        .HasConstraintName("fk_card_references_packs_pack_id");

                    b.Navigation("Card");
                });

            modelBuilder.Entity("TradeSaber.Models.Inventory", b =>
                {
                    b.HasOne("TradeSaber.Models.Card", null)
                        .WithMany("OwnedBy")
                        .HasForeignKey("CardID")
                        .HasConstraintName("fk_inventories_cards_card_id");
                });

            modelBuilder.Entity("TradeSaber.Models.Media", b =>
                {
                    b.HasOne("TradeSaber.Models.User", "Uploader")
                        .WithMany()
                        .HasForeignKey("UploaderID")
                        .HasConstraintName("fk_media_users_uploader_id");

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("TradeSaber.Models.Objective", b =>
                {
                    b.HasOne("TradeSaber.Models.Media", "Icon")
                        .WithMany()
                        .HasForeignKey("IconID")
                        .HasConstraintName("fk_objectives_media_icon_id");

                    b.Navigation("Icon");
                });

            modelBuilder.Entity("TradeSaber.Models.Objective+Result", b =>
                {
                    b.HasOne("TradeSaber.Models.Objective", "Objective")
                        .WithMany("ObjectiveResults")
                        .HasForeignKey("ObjectiveID")
                        .HasConstraintName("fk_objective_results_objectives_objective_id");

                    b.HasOne("TradeSaber.Models.User", "Submitter")
                        .WithMany("CompletedObjectives")
                        .HasForeignKey("SubmitterID")
                        .HasConstraintName("fk_objective_results_users_submitter_id");

                    b.Navigation("Objective");

                    b.Navigation("Submitter");
                });

            modelBuilder.Entity("TradeSaber.Models.Pack", b =>
                {
                    b.HasOne("TradeSaber.Models.Media", "Cover")
                        .WithMany()
                        .HasForeignKey("CoverID")
                        .HasConstraintName("fk_packs_media_cover_id");

                    b.Navigation("Cover");
                });

            modelBuilder.Entity("TradeSaber.Models.Pack+Reference", b =>
                {
                    b.HasOne("TradeSaber.Models.Inventory", null)
                        .WithMany("Packs")
                        .HasForeignKey("InventoryID")
                        .HasConstraintName("fk_pack_references_inventories_inventory_id");

                    b.HasOne("TradeSaber.Models.Objective", null)
                        .WithMany("PackRewards")
                        .HasForeignKey("ObjectiveID")
                        .HasConstraintName("fk_pack_references_objectives_objective_id");

                    b.HasOne("TradeSaber.Models.Pack", "Pack")
                        .WithMany()
                        .HasForeignKey("PackID1")
                        .HasConstraintName("fk_pack_references_packs_pack_id1");

                    b.HasOne("TradeSaber.Models.Rarity", null)
                        .WithMany("Packs")
                        .HasForeignKey("RarityID")
                        .HasConstraintName("fk_pack_references_rarities_rarity_id");

                    b.Navigation("Pack");
                });

            modelBuilder.Entity("TradeSaber.Models.Rarity+Reference", b =>
                {
                    b.HasOne("TradeSaber.Models.Pack", null)
                        .WithMany("Rarities")
                        .HasForeignKey("PackID")
                        .HasConstraintName("fk_rarity_references_packs_pack_id");

                    b.HasOne("TradeSaber.Models.Rarity", "Rarity")
                        .WithMany()
                        .HasForeignKey("RarityID")
                        .HasConstraintName("fk_rarity_references_rarities_rarity_id");

                    b.Navigation("Rarity");
                });

            modelBuilder.Entity("TradeSaber.Models.Series", b =>
                {
                    b.HasOne("TradeSaber.Models.Media", "Banner")
                        .WithMany()
                        .HasForeignKey("BannerID")
                        .HasConstraintName("fk_series_media_banner_id");

                    b.HasOne("TradeSaber.Models.Media", "Icon")
                        .WithMany()
                        .HasForeignKey("IconID")
                        .HasConstraintName("fk_series_media_icon_id");

                    b.Navigation("Banner");

                    b.Navigation("Icon");
                });

            modelBuilder.Entity("TradeSaber.Models.Series+Reference", b =>
                {
                    b.HasOne("TradeSaber.Models.Mutation", null)
                        .WithMany("Series")
                        .HasForeignKey("MutationID")
                        .HasConstraintName("fk_series_references_mutations_mutation_id");

                    b.HasOne("TradeSaber.Models.Series", "Series")
                        .WithMany()
                        .HasForeignKey("SeriesID1")
                        .HasConstraintName("fk_series_references_series_series_id1");

                    b.Navigation("Series");
                });

            modelBuilder.Entity("TradeSaber.Models.User", b =>
                {
                    b.HasOne("TradeSaber.Models.Inventory", "Inventory")
                        .WithMany()
                        .HasForeignKey("InventoryID")
                        .HasConstraintName("fk_users_inventories_inventory_id");

                    b.HasOne("TradeSaber.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleID")
                        .HasConstraintName("fk_users_roles_role_id");

                    b.Navigation("Inventory");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("TradeSaber.Models.Card", b =>
                {
                    b.Navigation("OwnedBy");
                });

            modelBuilder.Entity("TradeSaber.Models.Inventory", b =>
                {
                    b.Navigation("Cards");

                    b.Navigation("Packs");
                });

            modelBuilder.Entity("TradeSaber.Models.Mutation", b =>
                {
                    b.Navigation("Cards");

                    b.Navigation("Series");
                });

            modelBuilder.Entity("TradeSaber.Models.Objective", b =>
                {
                    b.Navigation("ObjectiveResults");

                    b.Navigation("PackRewards");
                });

            modelBuilder.Entity("TradeSaber.Models.Pack", b =>
                {
                    b.Navigation("CardPool");

                    b.Navigation("Rarities");
                });

            modelBuilder.Entity("TradeSaber.Models.Rarity", b =>
                {
                    b.Navigation("Packs");
                });

            modelBuilder.Entity("TradeSaber.Models.Series", b =>
                {
                    b.Navigation("Cards");
                });

            modelBuilder.Entity("TradeSaber.Models.User", b =>
                {
                    b.Navigation("CompletedObjectives");
                });
#pragma warning restore 612, 618
        }
    }
}
