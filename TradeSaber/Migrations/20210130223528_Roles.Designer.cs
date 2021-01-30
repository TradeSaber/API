﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TradeSaber;
using TradeSaber.Models.Discord;

namespace TradeSaber.Migrations
{
    [DbContext(typeof(TradeContext))]
    [Migration("20210130223528_Roles")]
    partial class Roles
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

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

            modelBuilder.Entity("TradeSaber.Models.User", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DiscordUser>("Profile")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("profile");

                    b.Property<Guid?>("RoleID")
                        .HasColumnType("uuid")
                        .HasColumnName("role_id");

                    b.HasKey("ID")
                        .HasName("pk_users");

                    b.HasIndex("RoleID")
                        .HasDatabaseName("ix_users_role_id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("TradeSaber.Models.User", b =>
                {
                    b.HasOne("TradeSaber.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleID")
                        .HasConstraintName("fk_users_roles_role_id");

                    b.Navigation("Role");
                });
#pragma warning restore 612, 618
        }
    }
}
