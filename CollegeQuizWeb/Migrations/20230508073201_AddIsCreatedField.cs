using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeQuizWeb.Migrations
{
    public partial class AddIsCreatedField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_created",
                table: "quiz_lobby",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_created",
                table: "quiz_lobby");
        }
    }
}
