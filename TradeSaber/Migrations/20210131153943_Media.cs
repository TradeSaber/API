using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeSaber.Migrations
{
    public partial class Media : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "ix_media_uploader_id",
                table: "media",
                column: "uploader_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "media");
        }
    }
}
