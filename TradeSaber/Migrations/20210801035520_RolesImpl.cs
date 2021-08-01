using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeSaber.Migrations
{
    public partial class RolesImpl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_role_role_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_role",
                table: "role");

            migrationBuilder.RenameTable(
                name: "role",
                newName: "roles");

            migrationBuilder.AddColumn<bool>(
                name: "root",
                table: "roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "pk_roles",
                table: "roles",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_roles_role_id",
                table: "users",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_roles_role_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_roles",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "root",
                table: "roles");

            migrationBuilder.RenameTable(
                name: "roles",
                newName: "role");

            migrationBuilder.AddPrimaryKey(
                name: "pk_role",
                table: "role",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_role_role_id",
                table: "users",
                column: "role_id",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
