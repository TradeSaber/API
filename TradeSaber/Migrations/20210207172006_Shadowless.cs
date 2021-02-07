using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeSaber.Migrations
{
    public partial class Shadowless : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pack_references_transactions_transaction_id",
                table: "pack_references");

            migrationBuilder.DropForeignKey(
                name: "fk_pack_references_transactions_transaction_id1",
                table: "pack_references");

            migrationBuilder.DropForeignKey(
                name: "fk_tradable_cards_cards_card_id1",
                table: "tradable_cards");

            migrationBuilder.DropForeignKey(
                name: "fk_tradable_cards_transactions_transaction_id",
                table: "tradable_cards");

            migrationBuilder.DropForeignKey(
                name: "fk_tradable_cards_transactions_transaction_id1",
                table: "tradable_cards");

            migrationBuilder.DropIndex(
                name: "ix_pack_references_transaction_id",
                table: "pack_references");

            migrationBuilder.DropIndex(
                name: "ix_pack_references_transaction_id1",
                table: "pack_references");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tradable_cards",
                table: "tradable_cards");

            migrationBuilder.DropIndex(
                name: "ix_tradable_cards_transaction_id1",
                table: "tradable_cards");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "pack_references");

            migrationBuilder.DropColumn(
                name: "transaction_id1",
                table: "pack_references");

            migrationBuilder.DropColumn(
                name: "transaction_id1",
                table: "tradable_cards");

            migrationBuilder.RenameTable(
                name: "tradable_cards",
                newName: "tradeable_cards");

            migrationBuilder.RenameIndex(
                name: "ix_tradable_cards_transaction_id",
                table: "tradeable_cards",
                newName: "ix_tradeable_cards_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_tradable_cards_card_id1",
                table: "tradeable_cards",
                newName: "ix_tradeable_cards_card_id1");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tradeable_cards",
                table: "tradeable_cards",
                column: "id");

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

            migrationBuilder.AddForeignKey(
                name: "fk_tradeable_cards_cards_card_id1",
                table: "tradeable_cards",
                column: "card_id1",
                principalTable: "cards",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tradeable_cards_transactions_transaction_id",
                table: "tradeable_cards",
                column: "transaction_id",
                principalTable: "transactions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tradeable_cards_cards_card_id1",
                table: "tradeable_cards");

            migrationBuilder.DropForeignKey(
                name: "fk_tradeable_cards_transactions_transaction_id",
                table: "tradeable_cards");

            migrationBuilder.DropTable(
                name: "tradeable_packs");

            migrationBuilder.DropTable(
                name: "tradeable_r_cards");

            migrationBuilder.DropTable(
                name: "tradeable_r_packs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tradeable_cards",
                table: "tradeable_cards");

            migrationBuilder.RenameTable(
                name: "tradeable_cards",
                newName: "tradable_cards");

            migrationBuilder.RenameIndex(
                name: "ix_tradeable_cards_transaction_id",
                table: "tradable_cards",
                newName: "ix_tradable_cards_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_tradeable_cards_card_id1",
                table: "tradable_cards",
                newName: "ix_tradable_cards_card_id1");

            migrationBuilder.AddColumn<Guid>(
                name: "transaction_id",
                table: "pack_references",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "transaction_id1",
                table: "pack_references",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "transaction_id1",
                table: "tradable_cards",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_tradable_cards",
                table: "tradable_cards",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_pack_references_transaction_id",
                table: "pack_references",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_pack_references_transaction_id1",
                table: "pack_references",
                column: "transaction_id1");

            migrationBuilder.CreateIndex(
                name: "ix_tradable_cards_transaction_id1",
                table: "tradable_cards",
                column: "transaction_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_pack_references_transactions_transaction_id",
                table: "pack_references",
                column: "transaction_id",
                principalTable: "transactions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pack_references_transactions_transaction_id1",
                table: "pack_references",
                column: "transaction_id1",
                principalTable: "transactions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tradable_cards_cards_card_id1",
                table: "tradable_cards",
                column: "card_id1",
                principalTable: "cards",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tradable_cards_transactions_transaction_id",
                table: "tradable_cards",
                column: "transaction_id",
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
        }
    }
}
