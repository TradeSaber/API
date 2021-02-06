using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeSaber.Migrations
{
    public partial class XP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "xp",
                table: "users",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xp",
                table: "users");
        }
    }
}
