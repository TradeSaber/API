using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeSaber.Migrations
{
    public partial class Transactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "transaction_id",
                table: "pack_references",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tir = table.Column<float>(type: "real", nullable: true),
                    state = table.Column<int>(type: "integer", nullable: false),
                    time_sent = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    time_acted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_transactions_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tradable_cards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    card_id1 = table.Column<Guid>(type: "uuid", nullable: true),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tradable_cards", x => x.id);
                    table.ForeignKey(
                        name: "fk_tradable_cards_cards_card_id1",
                        column: x => x.card_id1,
                        principalTable: "cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tradable_cards_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pack_references_transaction_id",
                table: "pack_references",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_tradable_cards_card_id1",
                table: "tradable_cards",
                column: "card_id1");

            migrationBuilder.CreateIndex(
                name: "ix_tradable_cards_transaction_id",
                table: "tradable_cards",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_sender_id",
                table: "transactions",
                column: "sender_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pack_references_transactions_transaction_id",
                table: "pack_references",
                column: "transaction_id",
                principalTable: "transactions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pack_references_transactions_transaction_id",
                table: "pack_references");

            migrationBuilder.DropTable(
                name: "tradable_cards");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropIndex(
                name: "ix_pack_references_transaction_id",
                table: "pack_references");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "pack_references");
        }
    }
}
