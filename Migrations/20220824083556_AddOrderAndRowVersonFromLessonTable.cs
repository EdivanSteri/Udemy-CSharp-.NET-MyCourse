using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCourse.Migrations
{
    public partial class AddOrderAndRowVersonFromLessonTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Lessons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1000);

            migrationBuilder.AddColumn<string>(
                name: "RowVersion",
                table: "Lessons",
                type: "TEXT",
                rowVersion: true,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Lessons");
        }
    }
}
