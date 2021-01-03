﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TradeSaber;
using TradeSaber.Models;
using TradeSaber.Models.Discord;
using static TradeSaber.Models.Session;

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
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("CardUser", b =>
                {
                    b.Property<Guid>("CardsID")
                        .HasColumnType("uuid")
                        .HasColumnName("cards_id");

                    b.Property<Guid>("OwnedByID")
                        .HasColumnType("uuid")
                        .HasColumnName("owned_by_id");

                    b.HasKey("CardsID", "OwnedByID")
                        .HasName("pk_card_user");

                    b.HasIndex("OwnedByID")
                        .HasDatabaseName("ix_card_user_owned_by_id");

                    b.ToTable("card_user");
                });

            modelBuilder.Entity("PackUser", b =>
                {
                    b.Property<Guid>("OwnedByID")
                        .HasColumnType("uuid")
                        .HasColumnName("owned_by_id");

                    b.Property<Guid>("PacksID")
                        .HasColumnType("uuid")
                        .HasColumnName("packs_id");

                    b.HasKey("OwnedByID", "PacksID")
                        .HasName("pk_pack_user");

                    b.HasIndex("PacksID")
                        .HasDatabaseName("ix_pack_user_packs_id");

                    b.ToTable("pack_user");
                });

            modelBuilder.Entity("TradeSaber.Models.Card", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("BaseURL")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("base_url");

                    b.Property<string>("CoverURL")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("cover_url");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("Locked")
                        .HasColumnType("boolean")
                        .HasColumnName("locked");

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

                    b.Property<int>("Rarity")
                        .HasColumnType("integer")
                        .HasColumnName("rarity");

                    b.Property<string>("Root")
                        .HasColumnType("text")
                        .HasColumnName("root");

                    b.Property<Guid>("SeriesID")
                        .HasColumnType("uuid")
                        .HasColumnName("series_id");

                    b.Property<Guid?>("TransactionID")
                        .HasColumnType("uuid")
                        .HasColumnName("transaction_id");

                    b.Property<float?>("Value")
                        .HasColumnType("real")
                        .HasColumnName("value");

                    b.HasKey("ID")
                        .HasName("pk_cards");

                    b.HasIndex("SeriesID")
                        .HasDatabaseName("ix_cards_series_id");

                    b.HasIndex("TransactionID")
                        .HasDatabaseName("ix_cards_transaction_id");

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

                    b.Property<Guid?>("CardID")
                        .HasColumnType("uuid")
                        .HasColumnName("card_id");

                    b.Property<bool>("Guaranteed")
                        .HasColumnType("boolean")
                        .HasColumnName("guaranteed");

                    b.Property<Guid?>("MutationID")
                        .HasColumnType("uuid")
                        .HasColumnName("mutation_id");

                    b.Property<Guid?>("PackID")
                        .HasColumnType("uuid")
                        .HasColumnName("pack_id");

                    b.HasKey("ID")
                        .HasName("pk_card_reference");

                    b.HasIndex("CardID")
                        .HasDatabaseName("ix_reference_card_id");

                    b.HasIndex("MutationID")
                        .HasDatabaseName("ix_reference_mutation_id");

                    b.HasIndex("PackID")
                        .HasDatabaseName("ix_reference_pack_id");

                    b.ToTable("card_reference");
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

            modelBuilder.Entity("TradeSaber.Models.Pack", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("CardID")
                        .HasColumnType("uuid")
                        .HasColumnName("card_id");

                    b.Property<int>("Count")
                        .HasColumnType("integer")
                        .HasColumnName("count");

                    b.Property<string>("CoverURL")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("cover_url");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<IList<Rarity>>("Rarities")
                        .HasColumnType("jsonb")
                        .HasColumnName("rarities");

                    b.Property<string>("Theme")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("theme");

                    b.Property<Guid?>("TransactionID")
                        .HasColumnType("uuid")
                        .HasColumnName("transaction_id");

                    b.Property<float?>("Value")
                        .HasColumnType("real")
                        .HasColumnName("value");

                    b.HasKey("ID")
                        .HasName("pk_packs");

                    b.HasIndex("CardID")
                        .HasDatabaseName("ix_packs_card_id");

                    b.HasIndex("TransactionID")
                        .HasDatabaseName("ix_packs_transaction_id");

                    b.ToTable("packs");
                });

            modelBuilder.Entity("TradeSaber.Models.Series", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("CoverURL")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("cover_url");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("MainColor")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("main_color");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("SubColor")
                        .HasColumnType("text")
                        .HasColumnName("sub_color");

                    b.HasKey("ID")
                        .HasName("pk_series");

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

                    b.Property<Guid?>("SeriesID")
                        .HasColumnType("uuid")
                        .HasColumnName("series_id");

                    b.HasKey("ID")
                        .HasName("pk_series_reference");

                    b.HasIndex("MutationID")
                        .HasDatabaseName("ix_reference_mutation_id1");

                    b.HasIndex("SeriesID")
                        .HasDatabaseName("ix_reference_series_id");

                    b.ToTable("series_reference");
                });

            modelBuilder.Entity("TradeSaber.Models.Session", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<float>("Length")
                        .HasColumnType("real")
                        .HasColumnName("length");

                    b.Property<Dictionary<Level, Score>>("Scores")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("scores");

                    b.Property<Instant>("StartTime")
                        .HasColumnType("timestamp")
                        .HasColumnName("start_time");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("ID")
                        .HasName("pk_sessions");

                    b.HasIndex("UserID")
                        .HasDatabaseName("ix_sessions_user_id");

                    b.ToTable("sessions");
                });

            modelBuilder.Entity("TradeSaber.Models.Transaction", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("ReceiverID")
                        .HasColumnType("uuid")
                        .HasColumnName("receiver_id");

                    b.Property<Guid?>("SenderID")
                        .HasColumnType("uuid")
                        .HasColumnName("sender_id");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<Instant>("Time")
                        .HasColumnType("timestamp")
                        .HasColumnName("time");

                    b.Property<float?>("Tir")
                        .HasColumnType("real")
                        .HasColumnName("tir");

                    b.HasKey("ID")
                        .HasName("pk_transactions");

                    b.HasIndex("ReceiverID")
                        .HasDatabaseName("ix_transactions_receiver_id");

                    b.HasIndex("SenderID")
                        .HasDatabaseName("ix_transactions_sender_id");

                    b.ToTable("transactions");
                });

            modelBuilder.Entity("TradeSaber.Models.User", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Instant>("Created")
                        .HasColumnType("timestamp")
                        .HasColumnName("created");

                    b.Property<long>("Experience")
                        .HasColumnType("bigint")
                        .HasColumnName("experience");

                    b.Property<Instant>("LastLoggedIn")
                        .HasColumnType("timestamp")
                        .HasColumnName("last_logged_in");

                    b.Property<int>("Level")
                        .HasColumnType("integer")
                        .HasColumnName("level");

                    b.Property<DiscordUser>("Profile")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("profile");

                    b.Property<int>("Role")
                        .HasColumnType("integer")
                        .HasColumnName("role");

                    b.Property<int>("State")
                        .HasColumnType("integer")
                        .HasColumnName("state");

                    b.Property<float>("TirCoin")
                        .HasColumnType("real")
                        .HasColumnName("tir_coin");

                    b.HasKey("ID")
                        .HasName("pk_users");

                    b.ToTable("users");
                });

            modelBuilder.Entity("CardUser", b =>
                {
                    b.HasOne("TradeSaber.Models.Card", null)
                        .WithMany()
                        .HasForeignKey("CardsID")
                        .HasConstraintName("fk_card_user_cards_cards_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TradeSaber.Models.User", null)
                        .WithMany()
                        .HasForeignKey("OwnedByID")
                        .HasConstraintName("fk_card_user_users_owned_by_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PackUser", b =>
                {
                    b.HasOne("TradeSaber.Models.User", null)
                        .WithMany()
                        .HasForeignKey("OwnedByID")
                        .HasConstraintName("fk_pack_user_users_owned_by_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TradeSaber.Models.Pack", null)
                        .WithMany()
                        .HasForeignKey("PacksID")
                        .HasConstraintName("fk_pack_user_packs_packs_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TradeSaber.Models.Card", b =>
                {
                    b.HasOne("TradeSaber.Models.Series", "Series")
                        .WithMany("Cards")
                        .HasForeignKey("SeriesID")
                        .HasConstraintName("fk_cards_series_series_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TradeSaber.Models.Transaction", null)
                        .WithMany("Cards")
                        .HasForeignKey("TransactionID")
                        .HasConstraintName("fk_cards_transactions_transaction_id");

                    b.Navigation("Series");
                });

            modelBuilder.Entity("TradeSaber.Models.Card+Reference", b =>
                {
                    b.HasOne("TradeSaber.Models.Card", "Card")
                        .WithMany()
                        .HasForeignKey("CardID")
                        .HasConstraintName("fk_reference_cards_card_id");

                    b.HasOne("TradeSaber.Models.Mutation", null)
                        .WithMany("Cards")
                        .HasForeignKey("MutationID")
                        .HasConstraintName("fk_reference_mutations_mutation_id");

                    b.HasOne("TradeSaber.Models.Pack", null)
                        .WithMany("CardPool")
                        .HasForeignKey("PackID")
                        .HasConstraintName("fk_reference_packs_pack_id");

                    b.Navigation("Card");
                });

            modelBuilder.Entity("TradeSaber.Models.Pack", b =>
                {
                    b.HasOne("TradeSaber.Models.Card", null)
                        .WithMany("Packs")
                        .HasForeignKey("CardID")
                        .HasConstraintName("fk_packs_cards_card_id");

                    b.HasOne("TradeSaber.Models.Transaction", null)
                        .WithMany("Packs")
                        .HasForeignKey("TransactionID")
                        .HasConstraintName("fk_packs_transactions_transaction_id");
                });

            modelBuilder.Entity("TradeSaber.Models.Series+Reference", b =>
                {
                    b.HasOne("TradeSaber.Models.Mutation", null)
                        .WithMany("Series")
                        .HasForeignKey("MutationID")
                        .HasConstraintName("fk_reference_mutations_mutation_id");

                    b.HasOne("TradeSaber.Models.Series", "Series")
                        .WithMany()
                        .HasForeignKey("SeriesID")
                        .HasConstraintName("fk_reference_series_series_id");

                    b.Navigation("Series");
                });

            modelBuilder.Entity("TradeSaber.Models.Session", b =>
                {
                    b.HasOne("TradeSaber.Models.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserID")
                        .HasConstraintName("fk_sessions_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("TradeSaber.Models.Transaction", b =>
                {
                    b.HasOne("TradeSaber.Models.User", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverID")
                        .HasConstraintName("fk_transactions_users_receiver_id");

                    b.HasOne("TradeSaber.Models.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderID")
                        .HasConstraintName("fk_transactions_users_sender_id");

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("TradeSaber.Models.Card", b =>
                {
                    b.Navigation("Packs");
                });

            modelBuilder.Entity("TradeSaber.Models.Mutation", b =>
                {
                    b.Navigation("Cards");

                    b.Navigation("Series");
                });

            modelBuilder.Entity("TradeSaber.Models.Pack", b =>
                {
                    b.Navigation("CardPool");
                });

            modelBuilder.Entity("TradeSaber.Models.Series", b =>
                {
                    b.Navigation("Cards");
                });

            modelBuilder.Entity("TradeSaber.Models.Transaction", b =>
                {
                    b.Navigation("Cards");

                    b.Navigation("Packs");
                });

            modelBuilder.Entity("TradeSaber.Models.User", b =>
                {
                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
