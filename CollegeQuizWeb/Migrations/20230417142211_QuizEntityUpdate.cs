using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeQuizWeb.Migrations
{
    public partial class QuizEntityUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "token_id",
                table: "quizes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_quizes_token_id",
                table: "quizes",
                column: "token_id");

            migrationBuilder.AddForeignKey(
                name: "FK_quizes_quiz_tokens_token_id",
                table: "quizes",
                column: "token_id",
                principalTable: "quiz_tokens",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quizes_quiz_tokens_token_id",
                table: "quizes");

            migrationBuilder.DropIndex(
                name: "IX_quizes_token_id",
                table: "quizes");

            migrationBuilder.DropColumn(
                name: "token_id",
                table: "quizes");
        }
    }
}
