using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPI_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherDisplayData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    StartTime = table.Column<string>(type: "TEXT", nullable: true),
                    StopTime = table.Column<string>(type: "TEXT", nullable: true),
                    Temperature = table.Column<string>(type: "TEXT", nullable: true),
                    WeatherCode = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherDisplayData", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "WeatherDisplayData",
                columns: new[] { "Id", "StartTime", "StopTime", "Temperature", "Text", "UpdatedAt", "WeatherCode" },
                values: new object[] { 1, "08:00", "18:00", "72°F", "", new DateTime(2025, 4, 6, 8, 0, 0, 0, DateTimeKind.Unspecified), "3" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherDisplayData");
        }
    }
}
