using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCourse.Migrations
{
    public partial class InizialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Courses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RowVersion",
                table: "Courses",
                type: "TEXT",
                rowVersion: true,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Courses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
