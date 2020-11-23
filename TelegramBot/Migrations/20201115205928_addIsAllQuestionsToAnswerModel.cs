using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramBot.Migrations
{
    public partial class addIsAllQuestionsToAnswerModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAllQuestions",
                table: "Answers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAllQuestions",
                table: "Answers");
        }
    }
}
