using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeSaber.Migrations
{
    public partial class Objectives : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pack_references_packs_pack_id",
                table: "pack_references");

            migrationBuilder.RenameColumn(
                name: "pack_id",
                table: "pack_references",
                newName: "pack_id1");

            migrationBuilder.RenameIndex(
                name: "ix_pack_references_pack_id",
                table: "pack_references",
                newName: "ix_pack_references_pack_id1");

            migrationBuilder.AddColumn<Guid>(
                name: "objective_id",
                table: "pack_references",
                type: "uuid",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "ix_pack_references_objective_id",
                table: "pack_references",
                column: "objective_id");

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

            migrationBuilder.AddForeignKey(
                name: "fk_pack_references_objectives_objective_id",
                table: "pack_references",
                column: "objective_id",
                principalTable: "objectives",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pack_references_packs_pack_id1",
                table: "pack_references",
                column: "pack_id1",
                principalTable: "packs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pack_references_objectives_objective_id",
                table: "pack_references");

            migrationBuilder.DropForeignKey(
                name: "fk_pack_references_packs_pack_id1",
                table: "pack_references");

            migrationBuilder.DropTable(
                name: "objective_results");

            migrationBuilder.DropTable(
                name: "objectives");

            migrationBuilder.DropIndex(
                name: "ix_pack_references_objective_id",
                table: "pack_references");

            migrationBuilder.DropColumn(
                name: "objective_id",
                table: "pack_references");

            migrationBuilder.RenameColumn(
                name: "pack_id1",
                table: "pack_references",
                newName: "pack_id");

            migrationBuilder.RenameIndex(
                name: "ix_pack_references_pack_id1",
                table: "pack_references",
                newName: "ix_pack_references_pack_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pack_references_packs_pack_id",
                table: "pack_references",
                column: "pack_id",
                principalTable: "packs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
