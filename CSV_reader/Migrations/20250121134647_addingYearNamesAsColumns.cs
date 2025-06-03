using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSV_reader.Migrations
{
    /// <inheritdoc />
    public partial class addingYearNamesAsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Year1Name",
                table: "ClientDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year2Name",
                table: "ClientDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year3Name",
                table: "ClientDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year4Name",
                table: "ClientDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year5Name",
                table: "ClientDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year1Name",
                table: "ClientDetails");

            migrationBuilder.DropColumn(
                name: "Year2Name",
                table: "ClientDetails");

            migrationBuilder.DropColumn(
                name: "Year3Name",
                table: "ClientDetails");

            migrationBuilder.DropColumn(
                name: "Year4Name",
                table: "ClientDetails");

            migrationBuilder.DropColumn(
                name: "Year5Name",
                table: "ClientDetails");
        }
    }
}
