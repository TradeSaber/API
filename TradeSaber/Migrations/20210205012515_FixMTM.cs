using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeSaber.Migrations
{
    public partial class FixMTM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_packs_inventories_inventory_id",
                table: "packs");

            migrationBuilder.DropForeignKey(
                name: "fk_rarities_packs_pack_id",
                table: "rarities");

            migrationBuilder.DropIndex(
                name: "ix_rarities_pack_id",
                table: "rarities");

            migrationBuilder.DropIndex(
                name: "ix_packs_inventory_id",
                table: "packs");

            migrationBuilder.DropColumn(
                name: "pack_id",
                table: "rarities");

            migrationBuilder.DropColumn(
                name: "inventory_id",
                table: "packs");

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
                name: "ix_inventory_pack_packs_id",
                table: "inventory_pack",
                column: "packs_id");

            migrationBuilder.CreateIndex(
                name: "ix_pack_rarity_rarities_id",
                table: "pack_rarity",
                column: "rarities_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventory_pack");

            migrationBuilder.DropTable(
                name: "pack_rarity");

            migrationBuilder.AddColumn<Guid>(
                name: "pack_id",
                table: "rarities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "inventory_id",
                table: "packs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_rarities_pack_id",
                table: "rarities",
                column: "pack_id");

            migrationBuilder.CreateIndex(
                name: "ix_packs_inventory_id",
                table: "packs",
                column: "inventory_id");

            migrationBuilder.AddForeignKey(
                name: "fk_packs_inventories_inventory_id",
                table: "packs",
                column: "inventory_id",
                principalTable: "inventories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rarities_packs_pack_id",
                table: "rarities",
                column: "pack_id",
                principalTable: "packs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
