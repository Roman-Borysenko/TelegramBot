using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramBot.Migrations
{
    public partial class addDepthForCategoryAndIsFileForAnswerModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Answers_CategoryId",
                table: "Answers");

            migrationBuilder.AddColumn<int>(
                name: "Depth",
                table: "Categories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFile",
                table: "Answers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_CategoryId",
                table: "Answers",
                column: "CategoryId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Answers_CategoryId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsFile",
                table: "Answers");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_CategoryId",
                table: "Answers",
                column: "CategoryId");
        }
    }
}
