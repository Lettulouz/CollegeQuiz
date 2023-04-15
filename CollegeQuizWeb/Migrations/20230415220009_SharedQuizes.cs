using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeQuizWeb.Migrations
{
    public partial class SharedQuizes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharedQuizesEnumerable",
                columns: table => new
                {
                    quiz_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedQuizesEnumerable", x => new { x.quiz_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_SharedQuizesEnumerable_quizes_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "quizes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SharedQuizesEnumerable_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SharedQuizesEnumerable_user_id",
                table: "SharedQuizesEnumerable",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedQuizesEnumerable");
        }
    }
}
