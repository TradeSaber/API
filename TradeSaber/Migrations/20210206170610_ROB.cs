using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeSaber.Migrations
{
    public partial class ROB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_inventories_cards_card_id",
                table: "inventories");

            migrationBuilder.DropIndex(
                name: "ix_inventories_card_id",
                table: "inventories");

            migrationBuilder.DropColumn(
                name: "card_id",
                table: "inventories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "card_id",
                table: "inventories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_inventories_card_id",
                table: "inventories",
                column: "card_id");

            migrationBuilder.AddForeignKey(
                name: "fk_inventories_cards_card_id",
                table: "inventories",
                column: "card_id",
                principalTable: "cards",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
