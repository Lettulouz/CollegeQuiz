using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeQuizWeb.Migrations
{
    public partial class CreateSharedQuizesRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                    name: "shared_quizes",
                    columns: table => new
                    {
                        quiz_id = table.Column<long>(type: "bigint", nullable: true),
                        user_id = table.Column<long>(type: "bigint", nullable: true),
                    },
                    constraints: table =>
                    {
                        table.ForeignKey(
                            name: "FK_shared_quizes_quizes_quiz_id",
                            column: x => x.quiz_id,
                            principalTable: "quizes",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade);
                        table.ForeignKey(
                            name: "FK_shared_quizes_users_user_id",
                            column: x => x.user_id,
                            principalTable: "users",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade);
                    })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
