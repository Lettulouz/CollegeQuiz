using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeQuizWeb.Migrations
{
    public partial class RangeAnswerMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "range_answer",
                table: "users_questions_answers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "range_answer",
                table: "users_questions_answers");
        }
    }
}
