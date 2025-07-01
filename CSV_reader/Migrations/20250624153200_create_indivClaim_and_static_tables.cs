using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSV_reader.Migrations
{
    /// <inheritdoc />
    public partial class create_indivClaim_and_static_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IndivClaimData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PolicyYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaimRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LossDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportedDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Registration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Make = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncidentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AD_Paid = table.Column<double>(type: "float", nullable: false),
                    FT_Paid = table.Column<double>(type: "float", nullable: false),
                    TPPD_Paid = table.Column<double>(type: "float", nullable: false),
                    TPCH_Paid = table.Column<double>(type: "float", nullable: false),
                    TPPI_Paid = table.Column<double>(type: "float", nullable: false),
                    ADOS = table.Column<double>(type: "float", nullable: false),
                    FTOS = table.Column<double>(type: "float", nullable: false),
                    TPPD_OS = table.Column<double>(type: "float", nullable: false),
                    TPCH_OS = table.Column<double>(type: "float", nullable: false),
                    TPPI_OS = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    RDaysCOI = table.Column<int>(type: "int", nullable: false),
                    RDaysNonCOI = table.Column<int>(type: "int", nullable: false),
                    TurnoverCOI = table.Column<double>(type: "float", nullable: false),
                    TurnoverNonCOI = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndivClaimData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaticClientDataDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ForecastTO_COI = table.Column<double>(type: "float", nullable: false),
                    ForecastTO_NonCOI = table.Column<double>(type: "float", nullable: false),
                    ForecastDaysCOI = table.Column<int>(type: "int", nullable: false),
                    ForecastDaysNonCOI = table.Column<int>(type: "int", nullable: false),
                    CarNums = table.Column<int>(type: "int", nullable: false),
                    VanNums = table.Column<int>(type: "int", nullable: false),
                    MinibusNums = table.Column<int>(type: "int", nullable: false),
                    HGVNums = table.Column<int>(type: "int", nullable: false),
                    ClientCoverType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientExcess = table.Column<double>(type: "float", nullable: false),
                    ClientStartDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientEndDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientCarLLL = table.Column<double>(type: "float", nullable: false),
                    ClientVanLLL = table.Column<double>(type: "float", nullable: false),
                    ClientMBusLLL = table.Column<double>(type: "float", nullable: false),
                    ClientHGVLLL = table.Column<double>(type: "float", nullable: false),
                    CarExposure = table.Column<double>(type: "float", nullable: false),
                    VanExposure = table.Column<double>(type: "float", nullable: false),
                    MinibusExposure = table.Column<double>(type: "float", nullable: false),
                    HGVExposure = table.Column<double>(type: "float", nullable: false),
                    ExpoPercentage = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticClientDataDB", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndivClaimData");

            migrationBuilder.DropTable(
                name: "StaticClientDataDB");
        }
    }
}
