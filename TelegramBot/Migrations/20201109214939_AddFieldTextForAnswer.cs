using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramBot.Migrations
{
    public partial class AddFieldTextForAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Answers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Answers");
        }
    }
}
