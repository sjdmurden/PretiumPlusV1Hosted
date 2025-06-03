using Microsoft.AspNetCore.Mvc.Rendering;

namespace CSV_reader.Models
{
    public class CalculationsModel
    {
        // ClientName
        public string ClientName { get; set; } = "";
        public string QuoteNumber { get; set; } = "";

        // Accident Freq
        public double Y1AccFreq { get; set; }
        public double Y2AccFreq { get; set; }
        public double Y3AccFreq { get; set; }
        public double Y4AccFreq { get; set; }
        public double Y5AccFreq { get; set; }
        public double ThreeY_AccFreq { get; set; }
        public double FiveY_AccFreq { get; set; }

        // Cost Per Claim
        public double Y1CPC { get; set; }
        public double Y2CPC { get; set; }
        public double Y3CPC { get; set; }
        public double Y4CPC { get; set; }
        public double Y5CPC { get; set; }
        public double ThreeY_CPC { get; set; }
        public double FiveY_CPC { get; set; }

        // Cost Per Insurable Day
        public double Y1CPID { get; set; }
        public double Y2CPID { get; set; }
        public double Y3CPID { get; set; }
        public double Y4CPID { get; set; }
        public double Y5CPID { get; set; }
        public double ThreeY_CPID { get; set; }
        public double FiveY_CPID { get; set; }

        // Claims Cost by TurnOver
        public double Y1CCTO { get; set; }
        public double Y2CCTO { get; set; }
        public double Y3CCTO { get; set; }
        public double Y4CCTO { get; set; }
        public double Y5CCTO { get; set; }
        public double ThreeY_CCTO { get; set; }
        public double FiveY_CCTO { get; set; }

        // Claims Cost per Vehicle Year AD 
        public double Y1_CCPVY_AD { get; set; }
        public double Y2_CCPVY_AD { get; set; }
        public double Y3_CCPVY_AD { get; set; }
        public double Y4_CCPVY_AD { get; set; }
        public double Y5_CCPVY_AD { get; set; }
        public double ThreeY_CCPVY_AD { get; set; }
        public double FiveY_CCPVY_AD { get; set; }

        // Claims Cost per Vehicle Year TP 
        public double Y1_CCPVY_TP { get; set; }
        public double Y2_CCPVY_TP { get; set; }
        public double Y3_CCPVY_TP { get; set; }
        public double Y4_CCPVY_TP { get; set; }
        public double Y5_CCPVY_TP { get; set; }
        public double ThreeY_CCPVY_TP { get; set; }
        public double FiveY_CCPVY_TP { get; set; }

        // Total CCPVY
        public double Y1_TotalCCPVY { get; set; }
        public double Y2_TotalCCPVY { get; set; }
        public double Y3_TotalCCPVY { get; set; }
        public double Y4_TotalCCPVY { get; set; }
        public double Y5_TotalCCPVY { get; set; }
        public double ThreeY_TotalCCPVY { get; set; }
        public double FiveY_TotalCCPVY { get; set; }

        public List<CalculationsYearsData> CalculationsYears { get; set; } = new List<CalculationsYearsData>();


        // --------------- INFLATION ----------------------------------------

        public List<CalculationsYearsDataInflated> CalculationsYearsInflated { get; set; } = new List<CalculationsYearsDataInflated>();

        // Inflation Months
        

        // CPC Inf
        public double Y1CPC_Inf { get; set; }
        public double Y2CPC_Inf { get; set; }
        public double Y3CPC_Inf { get; set; }
        public double Y4CPC_Inf { get; set; }
        public double Y5CPC_Inf { get; set; }
        public double ThreeY_CPC_Inf { get; set; }
        public double FiveY_CPC_Inf { get; set; }

        // CPID Inf
        public double Y1CPID_Inf { get; set; }
        public double Y2CPID_Inf { get; set; }
        public double Y3CPID_Inf { get; set; }
        public double Y4CPID_Inf { get; set; }
        public double Y5CPID_Inf { get; set; }
        public double ThreeY_CPID_Inf { get; set; }
        public double FiveY_CPID_Inf { get; set; }

        // CCTO Inf
        public double Y1CCTO_Inf { get; set; }
        public double Y2CCTO_Inf { get; set; }
        public double Y3CCTO_Inf { get; set; }
        public double Y4CCTO_Inf { get; set; }
        public double Y5CCTO_Inf { get; set; }
        public double ThreeY_CCTO_Inf { get; set; }
        public double FiveY_CCTO_Inf { get; set; }

        // CCPVY AD Inf
        public double Y1_CCPVYAD_Inf { get; set; }
        public double Y2_CCPVYAD_Inf { get; set; }
        public double Y3_CCPVYAD_Inf { get; set; }
        public double Y4_CCPVYAD_Inf { get; set; }
        public double Y5_CCPVYAD_Inf { get; set; }
        public double ThreeY_CCPVYAD_Inf { get; set; }
        public double FiveY_CCPVYAD_Inf { get; set; }

        // CCPVY TP Inf
        public double Y1_CCPVYTP_Inf { get; set; }
        public double Y2_CCPVYTP_Inf { get; set; }
        public double Y3_CCPVYTP_Inf { get; set; }
        public double Y4_CCPVYTP_Inf { get; set; }
        public double Y5_CCPVYTP_Inf { get; set; }
        public double ThreeY_CCPVYTP_Inf { get; set; }
        public double FiveY_CCPVYTP_Inf { get; set; }

        // Total CCPVY
        public double Y1_TotalCCPVY_Inf { get; set; }
        public double Y2_TotalCCPVY_Inf { get; set; }
        public double Y3_TotalCCPVY_Inf { get; set; }
        public double Y4_TotalCCPVY_Inf { get; set; }
        public double Y5_TotalCCPVY_Inf { get; set; }
        public double ThreeY_TotalCCPVY_Inf { get; set; }
        public double FiveY_TotalCCPVY_Inf { get; set; }
        // ----------------------------------------------------------------------------------


        public double TotalExposure { get; set; }
        public double TotalNonCOIExposure { get; set; }
        public double CC1000DaysBook { get; set; }
        public double CC1000DaysClient { get; set; }
        public double Variance { get; set; }
        public double NewExposure { get; set; }


        // ---------- TECHNICAL PRICE POINTS -------------------------------

        

        public double ProjectedClaimsTech { get; set; }
        public double ProjectedCCPVYTech { get; set; }
        public double ProjectedCPIRDTech { get; set; }
        public double ProjectedIBNRTech { get; set; }
        public double ProjectedExposureTech { get; set; }


        // ------------- ULTIMATE COSTS --------------------
        public List<SelectListItem> InflationMonths { get; set; } = new List<SelectListItem>();
        public int InfMonth { get; set; }
        public string SelectedNumOfMonths { get; set; } = "12";

        public List<SelectListItem> ProjectedYears { get; set; } = new List<SelectListItem>();
        public int ProjYear { get; set; }
        public string SelectedProjYears { get; set; } = "3";

        public List<SelectListItem> PriceByFilters { get; set; } = new List<SelectListItem>();
        public int PriceByValue { get; set; }
        public string SelectedPriceBy { get; set; } = "Experience";

        public List<SelectListItem> PricingMetrics { get; set; } = new List<SelectListItem>();
        public int PricingMetricValue { get; set; }
        public string SelectedPricingMetric { get; set; } = "CCPVY";

        public List<SelectListItem> ChargeCOIFee { get; set; } = new List<SelectListItem>();
        public int ChargeCOIFeeValue { get; set; }
        public string SelectedCOIFee { get; set; } = "Yes";

        public double ProjClaimsAmount { get; set; }
        public double ProjLLFund { get; set; }
        public double COI_Contingent { get; set; }
        public double ProjIBNR { get; set; }
        public double ProjExposure { get; set; }
        public double ProjClaimsHandlingFee { get; set; }
        public double Levies { get; set; }
        public double PretiumExpenses { get; set; }
        public double Profit { get; set; }
        public double ReinsuranceCosts { get; set; }
        public double Commission { get; set; } // was URG
        public double NetPremium { get; set; }
        public double GrossPremium { get; set; }
        public double GrossPremiumPlusIPT { get; set; }


        // ------------ historic years data props ----------------------------
        // the following properties are identical to the props in the HistoricYearsData class in the ClaimsModel but I did this in order to show the historic data table on both the Index and IndexCalculations pages.
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

    }

    /*public class CalculationsYearsData
    {
        public string Label { get; set; } = "";
        public string Year1 { get; set; }
        public string Year2 { get; set; }
        public string Year3 { get; set; }
        public string Year4 { get; set; }
        public string Year5 { get; set; }
        public string ThreeYearTotal { get; set; }
        public string FiveYearTotal { get; set; }
    }

    public class CalculationsYearsDataInflated
    {
        public string Label { get; set; } = "";
        public string Year1 { get; set; }
        public string Year2 { get; set; }
        public string Year3 { get; set; }
        public string Year4 { get; set; }
        public string Year5 { get; set; }
        public string ThreeYearAverage { get; set; }
        public string FiveYearAverage { get; set; }
    }*/
}
