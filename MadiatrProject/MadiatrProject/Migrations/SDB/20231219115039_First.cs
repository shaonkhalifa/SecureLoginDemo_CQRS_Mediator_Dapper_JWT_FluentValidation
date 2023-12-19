using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadiatrProject.Migrations.SDB
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CourseName = table.Column<string>(type: "TEXT", nullable: true),
                    CourseDescription = table.Column<string>(type: "TEXT", nullable: true),
                    Durations = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.CourseId);
                });

            migrationBuilder.CreateTable(
                name: "SessionTbl",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExpireTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LogInTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: true),
                    RoleID = table.Column<int>(type: "INTEGER", nullable: true),
                    UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    Permission = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionTbl", x => x.SessionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "SessionTbl");
        }
    }
}
