using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeQuizWeb.Migrations
{
    public partial class quizLobby : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quiz_lobby_users_user_host_id",
                table: "quiz_lobby");

            migrationBuilder.DropIndex(
                name: "IX_quiz_lobby_user_host_id",
                table: "quiz_lobby");

            migrationBuilder.DropColumn(
                name: "user_host_id",
                table: "quiz_lobby");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "user_host_id",
                table: "quiz_lobby",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_quiz_lobby_user_host_id",
                table: "quiz_lobby",
                column: "user_host_id");

            migrationBuilder.AddForeignKey(
                name: "FK_quiz_lobby_users_user_host_id",
                table: "quiz_lobby",
                column: "user_host_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
