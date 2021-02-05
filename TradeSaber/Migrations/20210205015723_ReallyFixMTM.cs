using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeSaber.Migrations
{
    public partial class ReallyFixMTM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "card_inventory");

            migrationBuilder.DropTable(
                name: "inventory_pack");

            migrationBuilder.DropTable(
                name: "pack_rarity");

            migrationBuilder.AddColumn<Guid>(
                name: "card_id",
                table: "inventories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "inventory_id",
                table: "card_references",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "pack_references",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pack_id = table.Column<Guid>(type: "uuid", nullable: true),
                    inventory_id = table.Column<Guid>(type: "uuid", nullable: true),
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
                        name: "fk_pack_references_packs_pack_id",
                        column: x => x.pack_id,
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

            migrationBuilder.CreateIndex(
                name: "ix_inventories_card_id",
                table: "inventories",
                column: "card_id");

            migrationBuilder.CreateIndex(
                name: "ix_card_references_inventory_id",
                table: "card_references",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "ix_pack_references_inventory_id",
                table: "pack_references",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "ix_pack_references_pack_id",
                table: "pack_references",
                column: "pack_id");

            migrationBuilder.CreateIndex(
                name: "ix_pack_references_rarity_id",
                table: "pack_references",
                column: "rarity_id");

            migrationBuilder.CreateIndex(
                name: "ix_rarity_references_pack_id",
                table: "rarity_references",
                column: "pack_id");

            migrationBuilder.CreateIndex(
                name: "ix_rarity_references_rarity_id",
                table: "rarity_references",
                column: "rarity_id");

            migrationBuilder.AddForeignKey(
                name: "fk_card_references_inventories_inventory_id",
                table: "card_references",
                column: "inventory_id",
                principalTable: "inventories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_inventories_cards_card_id",
                table: "inventories",
                column: "card_id",
                principalTable: "cards",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_card_references_inventories_inventory_id",
                table: "card_references");

            migrationBuilder.DropForeignKey(
                name: "fk_inventories_cards_card_id",
                table: "inventories");

            migrationBuilder.DropTable(
                name: "pack_references");

            migrationBuilder.DropTable(
                name: "rarity_references");

            migrationBuilder.DropIndex(
                name: "ix_inventories_card_id",
                table: "inventories");

            migrationBuilder.DropIndex(
                name: "ix_card_references_inventory_id",
                table: "card_references");

            migrationBuilder.DropColumn(
                name: "card_id",
                table: "inventories");

            migrationBuilder.DropColumn(
                name: "inventory_id",
                table: "card_references");

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
                name: "inventory_pack",
                columns: table => new
                {
                    in_inventories_id = table.Column<Guid>(type: "uuid", nullable: false),
                    packs_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_pack", x => new { x.in_inventories_id, x.packs_id });
                    table.ForeignKey(
                        name: "fk_inventory_pack_inventories_in_inventories_id",
                        column: x => x.in_inventories_id,
                        principalTable: "inventories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_inventory_pack_packs_packs_id",
                        column: x => x.packs_id,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pack_rarity",
                columns: table => new
                {
                    packs_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rarities_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pack_rarity", x => new { x.packs_id, x.rarities_id });
                    table.ForeignKey(
                        name: "fk_pack_rarity_packs_packs_id",
                        column: x => x.packs_id,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pack_rarity_rarities_rarities_id",
                        column: x => x.rarities_id,
                        principalTable: "rarities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_card_inventory_owned_by_id",
                table: "card_inventory",
                column: "owned_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_pack_packs_id",
                table: "inventory_pack",
                column: "packs_id");

            migrationBuilder.CreateIndex(
                name: "ix_pack_rarity_rarities_id",
                table: "pack_rarity",
                column: "rarities_id");
        }
    }
}
