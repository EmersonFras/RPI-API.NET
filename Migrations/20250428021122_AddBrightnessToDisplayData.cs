using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPI_API.Migrations
{
    public partial class AddBrightnessToDisplayData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brightness",
                table: "DisplayData",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brightness",
                table: "DisplayData");
        }
    }
}
