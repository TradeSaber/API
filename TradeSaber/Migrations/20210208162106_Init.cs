using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using TradeSaber.Models;
using TradeSaber.Models.Discord;

namespace TradeSaber.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "inventories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tir_coin = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mutations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    global_xp_boost = table.Column<float>(type: "real", nullable: true),
                    global_tir_boost = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mutations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rarities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    color = table.Column<string>(type: "text", nullable: false),
                    probability = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rarities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    scopes = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile = table.Column<DiscordUser>(type: "jsonb", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: true),
                    xp = table.Column<float>(type: "real", nullable: false),
                    inventory_id = table.Column<Guid>(type: "uuid", nullable: true),
                    settings = table.Column<Models.Settings>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_inventories_inventory_id",
                        column: x => x.inventory_id,
                        principalTable: "inventories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "media",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    mime_type = table.Column<string>(type: "text", nullable: false),
                    file_hash = table.Column<string>(type: "text", nullable: false),
                    uploader_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media", x => x.id);
                    table.ForeignKey(
                        name: "fk_media_users_uploader_id",
                        column: x => x.uploader_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tir = table.Column<float>(type: "real", nullable: true),
                    state = table.Column<int>(type: "integer", nullable: false),
                    time_sent = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    time_acted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    requested_tir = table.Column<float>(type: "real", nullable: true),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: true),
                    receiver_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_transactions_users_receiver_id",
                        column: x => x.receiver_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_transactions_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "objectives",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    icon_id = table.Column<Guid>(type: "uuid", nullable: true),
                    objective_type = table.Column<int>(type: "integer", nullable: false),
                    xp_reward = table.Column<float>(type: "real", nullable: true),
                    tir_reward = table.Column<float>(type: "real", nullable: true),
                    subject = table.Column<string>(type: "text", nullable: true),
                    template = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_objectives", x => x.id);
                    table.ForeignKey(
                        name: "fk_objectives_media_icon_id",
                        column: x => x.icon_id,
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "packs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    cover_id = table.Column<Guid>(type: "uuid", nullable: true),
                    value = table.Column<float>(type: "real", nullable: true),
                    card_count = table.Column<int>(type: "integer", nullable: false),
                    theme = table.Column<ColorTheme>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_packs", x => x.id);
                    table.ForeignKey(
                        name: "fk_packs_media_cover_id",
                        column: x => x.cover_id,
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "series",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    icon_id = table.Column<Guid>(type: "uuid", nullable: true),
                    banner_id = table.Column<Guid>(type: "uuid", nullable: true),
                    theme = table.Column<ColorTheme>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_series", x => x.id);
                    table.ForeignKey(
                        name: "fk_series_media_banner_id",
                        column: x => x.banner_id,
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_series_media_icon_id",
                        column: x => x.icon_id,
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "objective_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    submitter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    objective_id = table.Column<Guid>(type: "uuid", nullable: true),
                    submitted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modifier = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_objective_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_objective_results_objectives_objective_id",
                        column: x => x.objective_id,
                        principalTable: "objectives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_objective_results_users_submitter_id",
                        column: x => x.submitter_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pack_references",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pack_id1 = table.Column<Guid>(type: "uuid", nullable: true),
                    inventory_id = table.Column<Guid>(type: "uuid", nullable: true),
                    objective_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rarity_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pack_references", x => x.id);
                    table.ForeignKey(
                        name: "fk_pack_references_inventories_inventory_id",
                        column: x => x.inventory_id,
                        principalTable: "inventories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pack_references_objectives_objective_id",
                        column: x => x.objective_id,
                        principalTable: "objectives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pack_references_packs_pack_id1",
                        column: x => x.pack_id1,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pack_references_rarities_rarity_id",
                        column: x => x.rarity_id,
                        principalTable: "rarities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rarity_references",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rarity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pack_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rarity_references", x => x.id);
                    table.ForeignKey(
                        name: "fk_rarity_references_packs_pack_id",
                        column: x => x.pack_id,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rarity_references_rarities_rarity_id",
                        column: x => x.rarity_id,
                        principalTable: "rarities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tradeable_packs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pack_id1 = table.Column<Guid>(type: "uuid", nullable: true),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tradeable_packs", x => x.id);
                    table.ForeignKey(
                        name: "fk_tradeable_packs_packs_pack_id1",
                        column: x => x.pack_id1,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tradeable_packs_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tradeable_r_packs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pack_id1 = table.Column<Guid>(type: "uuid", nullable: true),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tradeable_r_packs", x => x.id);
                    table.ForeignKey(
                        name: "fk_tradeable_r_packs_packs_pack_id1",
                        column: x => x.pack_id1,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tradeable_r_packs_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    rarity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    @public = table.Column<bool>(name: "public", type: "boolean", nullable: false),
                    maximum = table.Column<int>(type: "integer", nullable: true),
                    value = table.Column<float>(type: "real", nullable: true),
                    probability = table.Column<float>(type: "real", nullable: false),
                    base_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cover_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cards", x => x.id);
                    table.ForeignKey(
                        name: "fk_cards_media_base_id",
                        column: x => x.base_id,
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cards_media_cover_id",
                        column: x => x.cover_id,
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cards_rarities_rarity_id",
                        column: x => x.rarity_id,
                        principalTable: "rarities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cards_series_series_id",
                        column: x => x.series_id,
                        principalTable: "series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "series_references",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    boost = table.Column<float>(type: "real", nullable: true),
                    series_id1 = table.Column<Guid>(type: "uuid", nullable: true),
                    mutation_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_series_references", x => x.id);
                    table.ForeignKey(
                        name: "fk_series_references_mutations_mutation_id",
                        column: x => x.mutation_id,
                        principalTable: "mutations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_series_references_series_series_id1",
                        column: x => x.series_id1,
                        principalTable: "series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "card_pack",
                columns: table => new
                {
                    cards_id = table.Column<Guid>(type: "uuid", nullable: false),
                    packs_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_pack", x => new { x.cards_id, x.packs_id });
                    table.ForeignKey(
                        name: "fk_card_pack_cards_cards_id",
                        column: x => x.cards_id,
                        principalTable: "cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_card_pack_packs_packs_id",
                        column: x => x.packs_id,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "card_references",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    guaranteed = table.Column<bool>(type: "boolean", nullable: false),
                    boost = table.Column<float>(type: "real", nullable: true),
                    card_id1 = table.Column<Guid>(type: "uuid", nullable: true),
                    mutation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pack_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_references", x => x.id);
                    table.ForeignKey(
                        name: "fk_card_references_cards_card_id1",
                        column: x => x.card_id1,
                        principalTable: "cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_card_references_mutations_mutation_id",
                        column: x => x.mutation_id,
                        principalTable: "mutations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_card_references_packs_pack_id",
                        column: x => x.pack_id,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tradeable_cards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    card_id1 = table.Column<Guid>(type: "uuid", nullable: true),
                    inventory_id = table.Column<Guid>(type: "uuid", nullable: true),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tradeable_cards", x => x.id);
                    table.ForeignKey(
                        name: "fk_tradeable_cards_cards_card_id1",
                        column: x => x.card_id1,
                        principalTable: "cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tradeable_cards_inventories_inventory_id",
                        column: x => x.inventory_id,
                        principalTable: "inventories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tradeable_cards_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tradeable_r_cards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    card_id1 = table.Column<Guid>(type: "uuid", nullable: true),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tradeable_r_cards", x => x.id);
                    table.ForeignKey(
                        name: "fk_tradeable_r_cards_cards_card_id1",
                        column: x => x.card_id1,
                        principalTable: "cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tradeable_r_cards_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_card_pack_packs_id",
                table: "card_pack",
                column: "packs_id");

            migrationBuilder.CreateIndex(
                name: "ix_card_references_card_id1",
                table: "card_references",
                column: "card_id1");

            migrationBuilder.CreateIndex(
                name: "ix_card_references_mutation_id",
                table: "card_references",
                column: "mutation_id");

            migrationBuilder.CreateIndex(
                name: "ix_card_references_pack_id",
                table: "card_references",
                column: "pack_id");

            migrationBuilder.CreateIndex(
                name: "ix_cards_base_id",
                table: "cards",
                column: "base_id");

            migrationBuilder.CreateIndex(
                name: "ix_cards_cover_id",
                table: "cards",
                column: "cover_id");

            migrationBuilder.CreateIndex(
                name: "ix_cards_rarity_id",
                table: "cards",
                column: "rarity_id");

            migrationBuilder.CreateIndex(
                name: "ix_cards_series_id",
                table: "cards",
                column: "series_id");

            migrationBuilder.CreateIndex(
                name: "ix_media_uploader_id",
                table: "media",
                column: "uploader_id");

            migrationBuilder.CreateIndex(
                name: "ix_objective_results_objective_id",
                table: "objective_results",
                column: "objective_id");

            migrationBuilder.CreateIndex(
                name: "ix_objective_results_submitter_id",
                table: "objective_results",
                column: "submitter_id");

            migrationBuilder.CreateIndex(
                name: "ix_objectives_icon_id",
                table: "objectives",
                column: "icon_id");

            migrationBuilder.CreateIndex(
                name: "ix_pack_references_inventory_id",
                table: "pack_references",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "ix_pack_references_objective_id",
                table: "pack_references",
                column: "objective_id");

            migrationBuilder.CreateIndex(
                name: "ix_pack_references_pack_id1",
                table: "pack_references",
                column: "pack_id1");

            migrationBuilder.CreateIndex(
                name: "ix_pack_references_rarity_id",
                table: "pack_references",
                column: "rarity_id");

            migrationBuilder.CreateIndex(
                name: "ix_packs_cover_id",
                table: "packs",
                column: "cover_id");

            migrationBuilder.CreateIndex(
                name: "ix_rarity_references_pack_id",
                table: "rarity_references",
                column: "pack_id");

            migrationBuilder.CreateIndex(
                name: "ix_rarity_references_rarity_id",
                table: "rarity_references",
                column: "rarity_id");

            migrationBuilder.CreateIndex(
                name: "ix_series_banner_id",
                table: "series",
                column: "banner_id");

            migrationBuilder.CreateIndex(
                name: "ix_series_icon_id",
                table: "series",
                column: "icon_id");

            migrationBuilder.CreateIndex(
                name: "ix_series_references_mutation_id",
                table: "series_references",
                column: "mutation_id");

            migrationBuilder.CreateIndex(
                name: "ix_series_references_series_id1",
                table: "series_references",
                column: "series_id1");

            migrationBuilder.CreateIndex(
                name: "ix_tradeable_cards_card_id1",
                table: "tradeable_cards",
                column: "card_id1");

            migrationBuilder.CreateIndex(
                name: "ix_tradeable_cards_inventory_id",
                table: "tradeable_cards",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "ix_tradeable_cards_transaction_id",
                table: "tradeable_cards",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_tradeable_packs_pack_id1",
                table: "tradeable_packs",
                column: "pack_id1");

            migrationBuilder.CreateIndex(
                name: "ix_tradeable_packs_transaction_id",
                table: "tradeable_packs",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_tradeable_r_cards_card_id1",
                table: "tradeable_r_cards",
                column: "card_id1");

            migrationBuilder.CreateIndex(
                name: "ix_tradeable_r_cards_transaction_id",
                table: "tradeable_r_cards",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_tradeable_r_packs_pack_id1",
                table: "tradeable_r_packs",
                column: "pack_id1");

            migrationBuilder.CreateIndex(
                name: "ix_tradeable_r_packs_transaction_id",
                table: "tradeable_r_packs",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_receiver_id",
                table: "transactions",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_sender_id",
                table: "transactions",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_inventory_id",
                table: "users",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                table: "users",
                column: "role_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "card_pack");

            migrationBuilder.DropTable(
                name: "card_references");

            migrationBuilder.DropTable(
                name: "objective_results");

            migrationBuilder.DropTable(
                name: "pack_references");

            migrationBuilder.DropTable(
                name: "rarity_references");

            migrationBuilder.DropTable(
                name: "series_references");

            migrationBuilder.DropTable(
                name: "tradeable_cards");

            migrationBuilder.DropTable(
                name: "tradeable_packs");

            migrationBuilder.DropTable(
                name: "tradeable_r_cards");

            migrationBuilder.DropTable(
                name: "tradeable_r_packs");

            migrationBuilder.DropTable(
                name: "objectives");

            migrationBuilder.DropTable(
                name: "mutations");

            migrationBuilder.DropTable(
                name: "cards");

            migrationBuilder.DropTable(
                name: "packs");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "rarities");

            migrationBuilder.DropTable(
                name: "series");

            migrationBuilder.DropTable(
                name: "media");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "inventories");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
