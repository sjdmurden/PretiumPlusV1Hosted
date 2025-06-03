namespace CSV_reader.Models
{
    public class ClientDetail
    {
        // client details page
        public int Id { get; set; }
        //public string QuoteNumber { get; set; }
        public string ClientName { get; set; }
        public string CoverType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public double Excess { get; set; }
        public int CarNums { get; set; } = 0;
        public int VanNums { get; set; } = 0;
        public int MinibusNums { get; set; } = 0;
        public int HGVNums { get; set; } = 0;
        public double CarLLL { get; set; } = 0;
        public double VanLLL { get; set; } = 0;
        public double MinibusLLL { get; set; } = 0;
        public double HGVLLL { get; set; } = 0;
        public double CarExp { get; set; } = 0;
        public double VanExp { get; set; } = 0;
        public double MinibusExp { get; set; } = 0;
        public double HGVExp { get; set; } = 0;
        public double ExpPercentage { get; set; } = 0;
        public int FCDaysCOI { get; set; } = 0;
        public int FCDaysNonCOI { get; set; } = 0;
        public double FCTO_COI { get; set; } = 0;
        public double FCTO_NonCOI { get; set; } = 0;
        // 
        // 
        //
        //
        // ----------------- historic data -----------------
        //
        public string? Year1Name { get; set; }
        public string? Year2Name { get; set; }
        public string? Year3Name { get; set; }
        public string? Year4Name { get; set; }
        public string? Year5Name { get; set; }
        public int Y1RentalDaysCOI { get; set; } = 0;
        public int Y2RentalDaysCOI { get; set; } = 0;
        public int Y3RentalDaysCOI { get; set; } = 0;
        public int Y4RentalDaysCOI { get; set; } = 0;
        public int Y5RentalDaysCOI { get; set; } = 0;
        public int ThreeY_RDaysCOI { get; set; } = 0;
        public int FiveY_RDaysCOI { get; set; } = 0;
        public int Y1RentalDaysNonCOI { get; set; } = 0;
        public int Y2RentalDaysNonCOI { get; set; } = 0;
        public int Y3RentalDaysNonCOI { get; set; } = 0;
        public int Y4RentalDaysNonCOI { get; set; } = 0;
        public int Y5RentalDaysNonCOI { get; set; } = 0;
        public int ThreeY_RDaysNonCOI { get; set; } = 0;
        public int FiveY_RDaysNonCOI { get; set; } = 0;
        // turnover COI/ Non COI
        public double Y1TO_COI { get; set; } = 0;
        public double Y2TO_COI { get; set; } = 0;
        public double Y3TO_COI { get; set; } = 0;
        public double Y4TO_COI { get; set; } = 0;
        public double Y5TO_COI { get; set; } = 0;
        public double ThreeY_TO_COI { get; set; } = 0;
        public double FiveY_TO_COI { get; set; } = 0;
        public double Y1TO_NonCOI { get; set; } = 0;
        public double Y2TO_NonCOI { get; set; } = 0;
        public double Y3TO_NonCOI { get; set; } = 0;
        public double Y4TO_NonCOI { get; set; } = 0;
        public double Y5TO_NonCOI { get; set; } = 0;
        public double ThreeY_TO_NonCOI { get; set; } = 0;
        public double FiveY_TO_NonCOI { get; set; } = 0;
        // utilisation rate
        public double Y1UT { get; set; } = 0;
        public double Y2UT { get; set; } = 0;
        public double Y3UT { get; set; } = 0;
        public double Y4UT { get; set; } = 0;
        public double Y5UT { get; set; } = 0;
        public double ThreeY_UT { get; set; } = 0;
        public double FiveY_UT { get; set; } = 0;
        // vehicle years
        public double Y1VYrs { get; set; } = 0;
        public double Y2VYrs { get; set; } = 0;
        public double Y3VYrs { get; set; } = 0;
        public double Y4VYrs { get; set; } = 0;
        public double Y5VYrs { get; set; } = 0;
        public double ThreeY_VYrs { get; set; } = 0;
        public double FiveY_VYrs { get; set; } = 0;
        // claims opened/ closed
        public int Y1ClaimsOpen { get; set; } = 0;
        public int Y2ClaimsOpen { get; set; } = 0;
        public int Y3ClaimsOpen { get; set; } = 0;
        public int Y4ClaimsOpen { get; set; } = 0;
        public int Y5ClaimsOpen { get; set; } = 0;
        public int ThreeY_ClaimsOpen { get; set; } = 0;
        public int FiveY_ClaimsOpen { get; set; } = 0;
        public int Y1ClaimsClosed { get; set; } = 0;
        public int Y2ClaimsClosed { get; set; } = 0;
        public int Y3ClaimsClosed { get; set; } = 0;
        public int Y4ClaimsClosed { get; set; } = 0;
        public int Y5ClaimsClosed { get; set; } = 0;
        public int ThreeY_ClaimsClo { get; set; } = 0;
        public int FiveY_ClaimsClo { get; set; } = 0;
        // AD Paid
        public double Y1ADPaid { get; set; } = 0;
        public double Y2ADPaid { get; set; } = 0;
        public double Y3ADPaid { get; set; } = 0;
        public double Y4ADPaid { get; set; } = 0;
        public double Y5ADPaid { get; set; } = 0;
        public double ThreeY_AD { get; set; } = 0;
        public double FiveY_AD { get; set; } = 0;
        // F&T Paid
        public double Y1FTPaid { get; set; } = 0;
        public double Y2FTPaid { get; set; } = 0;
        public double Y3FTPaid { get; set; } = 0;
        public double Y4FTPaid { get; set; } = 0;
        public double Y5FTPaid { get; set; } = 0;
        public double ThreeY_FT { get; set; } = 0;
        public double FiveY_FT { get; set; } = 0;
        // TP PD Paid
        public double Y1TPPD { get; set; } = 0;
        public double Y2TPPD { get; set; } = 0;
        public double Y3TPPD { get; set; } = 0;
        public double Y4TPPD { get; set; } = 0;
        public double Y5TPPD { get; set; } = 0;
        public double ThreeY_TPPD { get; set; } = 0;
        public double FiveY_TPPD { get; set; } = 0;
        // TP CH Paid
        public double Y1TPCH { get; set; } = 0;
        public double Y2TPCH { get; set; } = 0;
        public double Y3TPCH { get; set; } = 0;
        public double Y4TPCH { get; set; } = 0;
        public double Y5TPCH { get; set; } = 0;
        public double ThreeY_TPCH { get; set; } = 0;
        public double FiveY_TPCH { get; set; } = 0;
        // TP PI Paid
        public double Y1TPPI { get; set; } = 0;
        public double Y2TPPI { get; set; } = 0;
        public double Y3TPPI { get; set; } = 0;
        public double Y4TPPI { get; set; } = 0;
        public double Y5TPPI { get; set; } = 0;
        public double ThreeY_TPPI { get; set; } = 0;
        public double FiveY_TPPI { get; set; } = 0;
        // AD OS
        public double Y1ADOS { get; set; } = 0;
        public double Y2ADOS { get; set; } = 0;
        public double Y3ADOS { get; set; } = 0;
        public double Y4ADOS { get; set; } = 0;
        public double Y5ADOS { get; set; } = 0;
        public double ThreeY_ADOS { get; set; } = 0;
        public double FiveY_ADOS { get; set; } = 0;
        // F&T OS
        public double Y1FTOS { get; set; } = 0;
        public double Y2FTOS { get; set; } = 0;
        public double Y3FTOS { get; set; } = 0;
        public double Y4FTOS { get; set; } = 0;
        public double Y5FTOS { get; set; } = 0;
        public double ThreeY_FTOS { get; set; } = 0;
        public double FiveY_FTOS { get; set; } = 0;
        // TP PD OS
        public double Y1TPPDOS { get; set; } = 0;
        public double Y2TPPDOS { get; set; } = 0;
        public double Y3TPPDOS { get; set; } = 0;
        public double Y4TPPDOS { get; set; } = 0;
        public double Y5TPPDOS { get; set; } = 0;
        public double ThreeY_TPPDOS { get; set; } = 0;
        public double FiveY_TPPDOS { get; set; } = 0;
        // TP CH OS
        public double Y1TPCHOS { get; set; } = 0;
        public double Y2TPCHOS { get; set; } = 0;
        public double Y3TPCHOS { get; set; } = 0;
        public double Y4TPCHOS { get; set; } = 0;
        public double Y5TPCHOS { get; set; } = 0;
        public double ThreeY_TPCHOS { get; set; } = 0;
        public double FiveY_TPCHOS { get; set; } = 0;
        // TP PI OS
        public double Y1TPPIOS { get; set; } = 0;
        public double Y2TPPIOS { get; set; } = 0;
        public double Y3TPPIOS { get; set; } = 0;
        public double Y4TPPIOS { get; set; } = 0;
        public double Y5TPPIOS { get; set; } = 0;
        public double ThreeY_TPPIOS { get; set; } = 0;
        public double FiveY_TPPIOS { get; set; } = 0;
        // Total
        public double Y1Total { get; set; } = 0;
        public double Y2Total { get; set; } = 0;
        public double Y3Total { get; set; } = 0;
        public double Y4Total { get; set; } = 0;
        public double Y5Total { get; set; } = 0;
        public double ThreeY_Total { get; set; } = 0;
        public double FiveY_Total { get; set; } = 0;
    }
}
