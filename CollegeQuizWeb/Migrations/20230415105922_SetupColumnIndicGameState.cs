using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeQuizWeb.Migrations
{
    public partial class SetupColumnIndicGameState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_expired",
                table: "quiz_lobby");

            migrationBuilder.AddColumn<string>(
                name: "in_game_screen",
                table: "quiz_lobby",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "in_game_screen",
                table: "quiz_lobby");

            migrationBuilder.AddColumn<bool>(
                name: "is_expired",
                table: "quiz_lobby",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
