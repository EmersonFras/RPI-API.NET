using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPI_API.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisplayData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartTime = table.Column<string>(type: "TEXT", nullable: true),
                    StopTime = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherDisplayData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    Temperature = table.Column<string>(type: "TEXT", nullable: true),
                    WeatherCode = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherDisplayData", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DisplayData",
                columns: new[] { "Id", "StartTime", "StopTime", "UpdatedAt" },
                values: new object[] { 1, "08:00", "20:00", new DateTime(2025, 4, 6, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "WeatherDisplayData",
                columns: new[] { "Id", "Temperature", "Text", "UpdatedAt", "WeatherCode" },
                values: new object[] { 1, "72°F", "", new DateTime(2025, 4, 6, 8, 0, 0, 0, DateTimeKind.Unspecified), "3" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisplayData");

            migrationBuilder.DropTable(
                name: "WeatherDisplayData");
        }
    }
}
