using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeSaber.Migrations
{
    public partial class Mutations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reference_cards_card_id",
                table: "reference");

            migrationBuilder.DropForeignKey(
                name: "fk_reference_packs_pack_id",
                table: "reference");

            migrationBuilder.DropPrimaryKey(
                name: "pk_reference",
                table: "reference");

            migrationBuilder.RenameTable(
                name: "reference",
                newName: "card_references");

            migrationBuilder.RenameColumn(
                name: "card_id",
                table: "card_references",
                newName: "mutation_id");

            migrationBuilder.RenameIndex(
                name: "ix_reference_pack_id",
                table: "card_references",
                newName: "ix_card_references_pack_id");

            migrationBuilder.RenameIndex(
                name: "ix_reference_card_id",
                table: "card_references",
                newName: "ix_card_references_mutation_id");

            migrationBuilder.AddColumn<Guid>(
                name: "card_id1",
                table: "card_references",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_card_references",
                table: "card_references",
                column: "id");

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

            migrationBuilder.CreateIndex(
                name: "ix_card_references_card_id1",
                table: "card_references",
                column: "card_id1");

            migrationBuilder.CreateIndex(
                name: "ix_series_references_mutation_id",
                table: "series_references",
                column: "mutation_id");

            migrationBuilder.CreateIndex(
                name: "ix_series_references_series_id1",
                table: "series_references",
                column: "series_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_card_references_cards_card_id1",
                table: "card_references",
                column: "card_id1",
                principalTable: "cards",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_card_references_mutations_mutation_id",
                table: "card_references",
                column: "mutation_id",
                principalTable: "mutations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_card_references_packs_pack_id",
                table: "card_references",
                column: "pack_id",
                principalTable: "packs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_card_references_cards_card_id1",
                table: "card_references");

            migrationBuilder.DropForeignKey(
                name: "fk_card_references_mutations_mutation_id",
                table: "card_references");

            migrationBuilder.DropForeignKey(
                name: "fk_card_references_packs_pack_id",
                table: "card_references");

            migrationBuilder.DropTable(
                name: "series_references");

            migrationBuilder.DropTable(
                name: "mutations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_card_references",
                table: "card_references");

            migrationBuilder.DropIndex(
                name: "ix_card_references_card_id1",
                table: "card_references");

            migrationBuilder.DropColumn(
                name: "card_id1",
                table: "card_references");

            migrationBuilder.RenameTable(
                name: "card_references",
                newName: "reference");

            migrationBuilder.RenameColumn(
                name: "mutation_id",
                table: "reference",
                newName: "card_id");

            migrationBuilder.RenameIndex(
                name: "ix_card_references_pack_id",
                table: "reference",
                newName: "ix_reference_pack_id");

            migrationBuilder.RenameIndex(
                name: "ix_card_references_mutation_id",
                table: "reference",
                newName: "ix_reference_card_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_reference",
                table: "reference",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reference_cards_card_id",
                table: "reference",
                column: "card_id",
                principalTable: "cards",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_reference_packs_pack_id",
                table: "reference",
                column: "pack_id",
                principalTable: "packs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
