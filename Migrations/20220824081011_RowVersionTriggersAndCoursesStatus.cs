using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.Security.Cryptography;

#nullable disable

namespace MyCourse.Migrations
{
    public partial class RowVersionTriggersAndCoursesStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Courses",
                nullable: false,
                defaultValue: nameof(Models.Enums.CourseStatus.Draft));
            migrationBuilder.Sql($"UPDATE Courses SET Status='{nameof(Models.Enums.CourseStatus.Published)}'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "Status",
            table: "Courses");
        }
    }
}
