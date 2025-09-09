using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPI_API.Migrations
{
    public partial class AddDisplayUrlToImageData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayUrl",
                table: "ImageData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DisplayData",
                keyColumn: "Id",
                keyValue: 1,
                column: "Brightness",
                value: "100");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayUrl",
                table: "ImageData");

            migrationBuilder.UpdateData(
                table: "DisplayData",
                keyColumn: "Id",
                keyValue: 1,
                column: "Brightness",
                value: null);
        }
    }
}
