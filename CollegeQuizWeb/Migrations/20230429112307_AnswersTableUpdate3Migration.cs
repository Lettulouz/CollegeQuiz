using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeQuizWeb.Migrations
{
    public partial class AnswersTableUpdate3Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "step",
                table: "answers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "step",
                table: "answers");
        }
    }
}
