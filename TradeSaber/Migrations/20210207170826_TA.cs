using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeSaber.Migrations
{
    public partial class TA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "receiver_id",
                table: "transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "requested_tir",
                table: "transactions",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "transaction_id1",
                table: "tradable_cards",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "transaction_id1",
                table: "pack_references",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_receiver_id",
                table: "transactions",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "ix_tradable_cards_transaction_id1",
                table: "tradable_cards",
                column: "transaction_id1");

            migrationBuilder.CreateIndex(
                name: "ix_pack_references_transaction_id1",
                table: "pack_references",
                column: "transaction_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_pack_references_transactions_transaction_id1",
                table: "pack_references",
                column: "transaction_id1",
                principalTable: "transactions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tradable_cards_transactions_transaction_id1",
                table: "tradable_cards",
                column: "transaction_id1",
                principalTable: "transactions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_users_receiver_id",
                table: "transactions",
                column: "receiver_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pack_references_transactions_transaction_id1",
                table: "pack_references");

            migrationBuilder.DropForeignKey(
                name: "fk_tradable_cards_transactions_transaction_id1",
                table: "tradable_cards");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_users_receiver_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "ix_transactions_receiver_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "ix_tradable_cards_transaction_id1",
                table: "tradable_cards");

            migrationBuilder.DropIndex(
                name: "ix_pack_references_transaction_id1",
                table: "pack_references");

            migrationBuilder.DropColumn(
                name: "receiver_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "requested_tir",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "transaction_id1",
                table: "tradable_cards");

            migrationBuilder.DropColumn(
                name: "transaction_id1",
                table: "pack_references");
        }
    }
}
