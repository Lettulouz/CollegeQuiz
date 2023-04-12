using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeQuizWeb.Migrations
{
    public partial class FixedQuizSessionParticEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quiz_session_participants_quiz_lobby_QuizLobbyEntityId",
                table: "quiz_session_participants");

            migrationBuilder.DropIndex(
                name: "IX_quiz_session_participants_QuizLobbyEntityId",
                table: "quiz_session_participants");

            migrationBuilder.DropColumn(
                name: "QuizLobbyEntityId",
                table: "quiz_session_participants");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_session_participants_quiz_lobby_id",
                table: "quiz_session_participants",
                column: "quiz_lobby_id");

            migrationBuilder.AddForeignKey(
                name: "FK_quiz_session_participants_quiz_lobby_quiz_lobby_id",
                table: "quiz_session_participants",
                column: "quiz_lobby_id",
                principalTable: "quiz_lobby",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quiz_session_participants_quiz_lobby_quiz_lobby_id",
                table: "quiz_session_participants");

            migrationBuilder.DropIndex(
                name: "IX_quiz_session_participants_quiz_lobby_id",
                table: "quiz_session_participants");

            migrationBuilder.AddColumn<long>(
                name: "QuizLobbyEntityId",
                table: "quiz_session_participants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_quiz_session_participants_QuizLobbyEntityId",
                table: "quiz_session_participants",
                column: "QuizLobbyEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_quiz_session_participants_quiz_lobby_QuizLobbyEntityId",
                table: "quiz_session_participants",
                column: "QuizLobbyEntityId",
                principalTable: "quiz_lobby",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
