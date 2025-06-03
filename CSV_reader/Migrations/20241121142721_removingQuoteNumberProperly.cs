using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSV_reader.Migrations
{
    /// <inheritdoc />
    public partial class removingQuoteNumberProperly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuoteNumber",
                table: "ClientDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuoteNumber",
                table: "ClientDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
