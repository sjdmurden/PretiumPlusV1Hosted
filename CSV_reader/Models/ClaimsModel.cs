using Microsoft.AspNetCore.Mvc.Rendering;

namespace CSV_reader.Models
{
    public class ClaimsModel
    {
        //public string YearNameCol { get; set; }
        public string ClientNameCol { get; set; }
        public string PolicyYearCol { get; set; } //Column1
        public string ClaimReferenceCol { get; set; }
        public string LossDateCol { get; set; }
        public string ReportedDateCol { get; set; }
        public string RegCol { get; set; }
        public string MakeCol { get; set; }
        public string ModelCol { get; set; }
        public string VehicleTypeCol { get; set; }
        public string IncidentTypeCol { get; set; }
        public int RentalDaysCOICol { get; set; }
        public int RentalDaysNonCOICol { get; set; }
        public double TurnoverCOICol { get; set; }
        public double TurnoverNonCOICol { get; set; }
        public string StatusCol { get; set; } //Column11
        public double AD_PaidCol { get; set; } //Column12
        public double FT_PaidCol { get; set; } //Column13
        public double TPPD_PaidCol { get; set; } //Column14
        public double TPCH_PaidCol { get; set; } //Column15
        public double TPPI_PaidCol { get; set; } //Column16
        public double ADOSCol { get; set; } //Column18
        public double FTOSCol { get; set; } //Column19
        public double TPPD_OSCol { get; set; } //Column20
        public double TPCH_OSCol { get; set; } //Colum21
        public double TPPI_OSCol { get; set; } //Column22
        public double TotalCol { get; set; } //Column24

        public int CarNumsCol { get; set; }
        public int VanNumsCol { get; set; }
        public int MinibusNumsCol { get; set; }
        public int HGVNumsCol { get; set; }

        public double FCTO_COICol { get; set; }
        public double FCTO_NonCOICol { get; set; }
        public int FCDaysCOICol { get; set; }
        public int FCDaysNonCOICol { get; set; }
    }



    // this class is for each individual year and its totals
    public class PolicyYearSummary
    {
        //public string YearName { get; set; }
        public string PolicyYearName { get; set; }
        public string ClientName { get; set; }
        public int RentalDaysCOI { get; set; }
        public int RentalDaysNonCOI { get; set; }
        public double TurnoverCOI { get; set; }
        public double TurnoverNonCOI { get; set; }
        public int VehYears { get; set; }
        public int OpenClaims { get; set; } 
        public int ClosedClaims { get; set; } 
        public double Total_AD_Paid { get; set; }
        public double Total_FT_Paid { get; set; }
        public double Total_TPPD_Paid { get; set; }
        public double Total_TPCH_Paid { get; set; }
        public double Total_TPPI_Paid { get; set; }
        public double Total_AD_OS { get; set; }
        public double Total_FT_OS { get; set; }
        public double Total_TPPD_OS { get; set; }
        public double Total_TPCH_OS { get; set; }
        public double Total_TPPI_OS { get; set; }
        public double Total { get; set; }

        public int CarNums { get; set; }
        public int VanNums { get; set; }
        public int MinibusNums { get; set; }
        public int HGVNums { get; set; }

        public double CarLLL { get; set; }
        public double VanLLL { get; set; }
        public double MinibusLLL { get; set; }
        public double HGVLLL { get; set; }

        public double CarExposure { get; set; }
        public double VanExposure { get; set; }
        public double MinibusExposure { get; set; }
        public double HGVExposure { get; set; }

        public double FCTO_COI { get; set; }
        public double FCTO_NonCOI { get; set; }
        public int FCDaysCOI { get; set; }
        public int FCDaysNonCOI { get; set; }
    }


    //this class is for displaying each years' summarised data and also the rolling aggregates for three and five years
    public class HistoricYearsData
    {
        public string? Year1Name { get; set; }
        public string? Year2Name { get; set; }
        public string? Year3Name { get; set; }
        public string? Year4Name { get; set; }
        public string? Year5Name { get; set; }


        // rental days COI/ Non COI
        public int Y1RentalDaysCOI { get; set; }
        public int Y2RentalDaysCOI { get; set; }
        public int Y3RentalDaysCOI { get; set; }
        public int Y4RentalDaysCOI { get; set; }
        public int Y5RentalDaysCOI { get; set; }
        public int ThreeY_RDaysCOI { get; set; }
        public int FiveY_RDaysCOI { get; set; }
        public int Y1RentalDaysNonCOI { get; set; }
        public int Y2RentalDaysNonCOI { get; set; }
        public int Y3RentalDaysNonCOI { get; set; }
        public int Y4RentalDaysNonCOI { get; set; }
        public int Y5RentalDaysNonCOI { get; set; }
        public int ThreeY_RDaysNonCOI { get; set; }
        public int FiveY_RDaysNonCOI { get; set; }
        // turnover COI/ Non COI
        public double Y1TO_COI { get; set; }
        public double Y2TO_COI { get; set; }
        public double Y3TO_COI { get; set; }
        public double Y4TO_COI { get; set; }
        public double Y5TO_COI { get; set; }
        public double ThreeY_TO_COI { get; set; }
        public double FiveY_TO_COI { get; set; }
        public double Y1TO_NonCOI { get; set; }
        public double Y2TO_NonCOI { get; set; }
        public double Y3TO_NonCOI { get; set; }
        public double Y4TO_NonCOI { get; set; }
        public double Y5TO_NonCOI { get; set; }
        public double ThreeY_TO_NonCOI { get; set; }
        public double FiveY_TO_NonCOI { get; set; }

        // utilisation rate
        public double Y1UT { get; set; } = 0;
        public double Y2UT { get; set; } = 0;
        public double Y3UT { get; set; } = 0;
        public double Y4UT { get; set; } = 0;
        public double Y5UT { get; set; } = 0;
        public double ThreeY_UT { get; set; } = 0;
        public double FiveY_UT { get; set; } = 0;
        // vehicle years
        public double Y1VYrs { get; set; }
        public double Y2VYrs { get; set; }
        public double Y3VYrs { get; set; }
        public double Y4VYrs { get; set; }
        public double Y5VYrs { get; set; }
        public double ThreeY_VYrs { get; set; }
        public double FiveY_VYrs { get; set; }
        // claims opened/ closed
        public int Y1ClaimsOpen { get; set; }
        public int Y2ClaimsOpen { get; set; }
        public int Y3ClaimsOpen { get; set; }
        public int Y4ClaimsOpen { get; set; }
        public int Y5ClaimsOpen { get; set; }
        public int ThreeY_ClaimsOpen { get; set; }
        public int FiveY_ClaimsOpen { get; set; }
        public int Y1ClaimsClosed { get; set; }
        public int Y2ClaimsClosed { get; set; }
        public int Y3ClaimsClosed { get; set; }
        public int Y4ClaimsClosed { get; set; }
        public int Y5ClaimsClosed { get; set; }
        public int ThreeY_ClaimsClo { get; set; }
        public int FiveY_ClaimsClo { get; set; }
        // AD Paid
        public double Y1ADPaid { get; set; }
        public double Y2ADPaid { get; set; }
        public double Y3ADPaid { get; set; }
        public double Y4ADPaid { get; set; }
        public double Y5ADPaid { get; set; }
        public double ThreeY_AD { get; set; }
        public double FiveY_AD { get; set; }
        // F&T Paid
        public double Y1FTPaid { get; set; }
        public double Y2FTPaid { get; set; }
        public double Y3FTPaid { get; set; }
        public double Y4FTPaid { get; set; }
        public double Y5FTPaid { get; set; }
        public double ThreeY_FT { get; set; }
        public double FiveY_FT { get; set; }
        // TP PD Paid
        public double Y1TPPD { get; set; }
        public double Y2TPPD { get; set; }
        public double Y3TPPD { get; set; }
        public double Y4TPPD { get; set; }
        public double Y5TPPD { get; set; }
        public double ThreeY_TPPD { get; set; }
        public double FiveY_TPPD { get; set; }
        // TP CH Paid
        public double Y1TPCH { get; set; }
        public double Y2TPCH { get; set; }
        public double Y3TPCH { get; set; }
        public double Y4TPCH { get; set; }
        public double Y5TPCH { get; set; }
        public double ThreeY_TPCH { get; set; }
        public double FiveY_TPCH { get; set; }
        // TP PI Paid
        public double Y1TPPI { get; set; }
        public double Y2TPPI { get; set; }
        public double Y3TPPI { get; set; }
        public double Y4TPPI { get; set; }
        public double Y5TPPI { get; set; }
        public double ThreeY_TPPI { get; set; }
        public double FiveY_TPPI { get; set; }
        // AD OS
        public double Y1ADOS { get; set; }
        public double Y2ADOS { get; set; }
        public double Y3ADOS { get; set; }
        public double Y4ADOS { get; set; }
        public double Y5ADOS { get; set; }
        public double ThreeY_ADOS { get; set; }
        public double FiveY_ADOS { get; set; }
        // F&T OS
        public double Y1FTOS { get; set; }
        public double Y2FTOS { get; set; }
        public double Y3FTOS { get; set; }
        public double Y4FTOS { get; set; }
        public double Y5FTOS { get; set; }
        public double ThreeY_FTOS { get; set; }
        public double FiveY_FTOS { get; set; }
        // TP PD OS
        public double Y1TPPDOS { get; set; }
        public double Y2TPPDOS { get; set; }
        public double Y3TPPDOS { get; set; }
        public double Y4TPPDOS { get; set; }
        public double Y5TPPDOS { get; set; }
        public double ThreeY_TPPDOS { get; set; }
        public double FiveY_TPPDOS { get; set; }
        // TP CH OS
        public double Y1TPCHOS { get; set; }
        public double Y2TPCHOS { get; set; }
        public double Y3TPCHOS { get; set; }
        public double Y4TPCHOS { get; set; }
        public double Y5TPCHOS { get; set; }
        public double ThreeY_TPCHOS { get; set; }
        public double FiveY_TPCHOS { get; set; }
        // TP PI OS
        public double Y1TPPIOS { get; set; }
        public double Y2TPPIOS { get; set; }
        public double Y3TPPIOS { get; set; }
        public double Y4TPPIOS { get; set; }
        public double Y5TPPIOS { get; set; }
        public double ThreeY_TPPIOS { get; set; }
        public double FiveY_TPPIOS { get; set; }
        // Total
        public double Y1Total { get; set; }
        public double Y2Total { get; set; }
        public double Y3Total { get; set; }
        public double Y4Total { get; set; }
        public double Y5Total { get; set; }
        public double ThreeY_Total { get; set; }
        public double FiveY_Total { get; set; }

        // public List<HistoricYearsData> HistoricYears { get; set; } = new List<HistoricYearsData>();
    }
}
