using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeQuizWeb.Migrations
{
    public partial class DatabaseFix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_questions_question_types_question_type_id",
                table: "questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_question_types",
                table: "question_types");

            migrationBuilder.AlterColumn<long>(
                name: "question_type_id",
                table: "questions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "site_id",
                table: "question_types",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "question_types",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_question_types",
                table: "question_types",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_questions_question_types_question_type_id",
                table: "questions",
                column: "question_type_id",
                principalTable: "question_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_questions_question_types_question_type_id",
                table: "questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_question_types",
                table: "question_types");

            migrationBuilder.DropColumn(
                name: "id",
                table: "question_types");

            migrationBuilder.AlterColumn<int>(
                name: "question_type_id",
                table: "questions",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "site_id",
                table: "question_types",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_question_types",
                table: "question_types",
                column: "site_id");

            migrationBuilder.AddForeignKey(
                name: "FK_questions_question_types_question_type_id",
                table: "questions",
                column: "question_type_id",
                principalTable: "question_types",
                principalColumn: "site_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
