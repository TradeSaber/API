using System;
using Microsoft.EntityFrameworkCore.Migrations;
using TradeSaber.Models;

namespace TradeSaber.Migrations
{
    public partial class SCP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "inventory_id",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "pack_id",
                table: "rarities",
                type: "uuid",
                nullable: true);

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
                name: "packs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    cover_id = table.Column<Guid>(type: "uuid", nullable: true),
                    value = table.Column<float>(type: "real", nullable: true),
                    card_count = table.Column<int>(type: "integer", nullable: false),
                    theme = table.Column<ColorTheme>(type: "jsonb", nullable: false),
                    inventory_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_packs", x => x.id);
                    table.ForeignKey(
                        name: "fk_packs_inventories_inventory_id",
                        column: x => x.inventory_id,
                        principalTable: "inventories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_packs_media_cover_id",
                        column: x => x.cover_id,
                        principalTable: "media",
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
                name: "card_inventory",
                columns: table => new
                {
                    cards_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owned_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_inventory", x => new { x.cards_id, x.owned_by_id });
                    table.ForeignKey(
                        name: "fk_card_inventory_cards_cards_id",
                        column: x => x.cards_id,
                        principalTable: "cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_card_inventory_inventories_owned_by_id",
                        column: x => x.owned_by_id,
                        principalTable: "inventories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "reference",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    guaranteed = table.Column<bool>(type: "boolean", nullable: false),
                    card_id = table.Column<Guid>(type: "uuid", nullable: true),
                    boost = table.Column<float>(type: "real", nullable: true),
                    pack_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reference", x => x.id);
                    table.ForeignKey(
                        name: "fk_reference_cards_card_id",
                        column: x => x.card_id,
                        principalTable: "cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reference_packs_pack_id",
                        column: x => x.pack_id,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_users_inventory_id",
                table: "users",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "ix_rarities_pack_id",
                table: "rarities",
                column: "pack_id");

            migrationBuilder.CreateIndex(
                name: "ix_card_inventory_owned_by_id",
                table: "card_inventory",
                column: "owned_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_card_pack_packs_id",
                table: "card_pack",
                column: "packs_id");

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
                name: "ix_packs_cover_id",
                table: "packs",
                column: "cover_id");

            migrationBuilder.CreateIndex(
                name: "ix_packs_inventory_id",
                table: "packs",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "ix_reference_card_id",
                table: "reference",
                column: "card_id");

            migrationBuilder.CreateIndex(
                name: "ix_reference_pack_id",
                table: "reference",
                column: "pack_id");

            migrationBuilder.CreateIndex(
                name: "ix_series_banner_id",
                table: "series",
                column: "banner_id");

            migrationBuilder.CreateIndex(
                name: "ix_series_icon_id",
                table: "series",
                column: "icon_id");

            migrationBuilder.AddForeignKey(
                name: "fk_rarities_packs_pack_id",
                table: "rarities",
                column: "pack_id",
                principalTable: "packs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_users_inventories_inventory_id",
                table: "users",
                column: "inventory_id",
                principalTable: "inventories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rarities_packs_pack_id",
                table: "rarities");

            migrationBuilder.DropForeignKey(
                name: "fk_users_inventories_inventory_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "card_inventory");

            migrationBuilder.DropTable(
                name: "card_pack");

            migrationBuilder.DropTable(
                name: "reference");

            migrationBuilder.DropTable(
                name: "cards");

            migrationBuilder.DropTable(
                name: "packs");

            migrationBuilder.DropTable(
                name: "series");

            migrationBuilder.DropTable(
                name: "inventories");

            migrationBuilder.DropIndex(
                name: "ix_users_inventory_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_rarities_pack_id",
                table: "rarities");

            migrationBuilder.DropColumn(
                name: "inventory_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "pack_id",
                table: "rarities");
        }
    }
}
