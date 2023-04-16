using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeQuizWeb.Migrations
{
    public partial class TableSharedQuizes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedQuizesEnumerable_quizes_quiz_id",
                table: "SharedQuizesEnumerable");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedQuizesEnumerable_users_user_id",
                table: "SharedQuizesEnumerable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SharedQuizesEnumerable",
                table: "SharedQuizesEnumerable");

            migrationBuilder.RenameTable(
                name: "SharedQuizesEnumerable",
                newName: "shared_quizes");

            migrationBuilder.RenameIndex(
                name: "IX_SharedQuizesEnumerable_user_id",
                table: "shared_quizes",
                newName: "IX_shared_quizes_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shared_quizes",
                table: "shared_quizes",
                columns: new[] { "quiz_id", "user_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_shared_quizes_quizes_quiz_id",
                table: "shared_quizes",
                column: "quiz_id",
                principalTable: "quizes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_shared_quizes_users_user_id",
                table: "shared_quizes",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shared_quizes_quizes_quiz_id",
                table: "shared_quizes");

            migrationBuilder.DropForeignKey(
                name: "FK_shared_quizes_users_user_id",
                table: "shared_quizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_shared_quizes",
                table: "shared_quizes");

            migrationBuilder.RenameTable(
                name: "shared_quizes",
                newName: "SharedQuizesEnumerable");

            migrationBuilder.RenameIndex(
                name: "IX_shared_quizes_user_id",
                table: "SharedQuizesEnumerable",
                newName: "IX_SharedQuizesEnumerable_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharedQuizesEnumerable",
                table: "SharedQuizesEnumerable",
                columns: new[] { "quiz_id", "user_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_SharedQuizesEnumerable_quizes_quiz_id",
                table: "SharedQuizesEnumerable",
                column: "quiz_id",
                principalTable: "quizes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedQuizesEnumerable_users_user_id",
                table: "SharedQuizesEnumerable",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
