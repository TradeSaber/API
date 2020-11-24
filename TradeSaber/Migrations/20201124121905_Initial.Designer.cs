﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TradeSaber;
using TradeSaber.Models.Discord;

namespace TradeSaber.Migrations
{
    [DbContext(typeof(TradeContext))]
    [Migration("20201124121905_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0");

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

                    b.Property<float?>("Value")
                        .HasColumnType("real")
                        .HasColumnName("value");

                    b.HasKey("ID")
                        .HasName("pk_cards");

                    b.HasIndex("SeriesID")
                        .HasDatabaseName("ix_cards_series_id");

                    b.ToTable("cards");
                });

            modelBuilder.Entity("TradeSaber.Models.Series", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("BannerURL")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("banner_url");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("IconURL")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("icon_url");

                    b.Property<string>("MainColor")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("main_color");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("SubColor")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("sub_color");

                    b.HasKey("ID")
                        .HasName("pk_series");

                    b.ToTable("series");
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

            modelBuilder.Entity("TradeSaber.Models.Card", b =>
                {
                    b.HasOne("TradeSaber.Models.Series", "Series")
                        .WithMany("Cards")
                        .HasForeignKey("SeriesID")
                        .HasConstraintName("fk_cards_series_series_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Series");
                });

            modelBuilder.Entity("TradeSaber.Models.Series", b =>
                {
                    b.Navigation("Cards");
                });
#pragma warning restore 612, 618
        }
    }
}
