using Microsoft.EntityFrameworkCore.Migrations;
using TradeSaber.Models;

namespace TradeSaber.Migrations
{
    public partial class Settings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Settings>(
                name: "settings",
                table: "users",
                type: "jsonb",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "settings",
                table: "users");
        }
    }
}
