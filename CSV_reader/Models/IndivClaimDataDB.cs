namespace CSV_reader.Models
{
    public class IndivClaimDataDB
    {
        public int Id { get; set; }

        // Unique identifier for each uploaded file - each batch of claims will have this Id
        public string BatchId { get; set; }

        public string ClientName { get; set; }
        public string PolicyYear { get; set; }
        public string ClaimRef { get; set; }
        public string LossDate { get; set; }
        public string ReportedDate { get; set; }
        public string Registration { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string VehicleType { get; set; }
        public string IncidentType { get; set; }
        public string Status { get; set; }

        public double AD_Paid { get; set; }
        public double FT_Paid { get; set; }
        public double TPPD_Paid { get; set; }
        public double TPCH_Paid { get; set; }
        public double TPPI_Paid { get; set; }
        public double ADOS { get; set; }
        public double FTOS { get; set; }
        public double TPPD_OS { get; set; }
        public double TPCH_OS { get; set; }
        public double TPPI_OS { get; set; }
        public double Total { get; set; }

        public int RDaysCOI { get; set; }
        public int RDaysNonCOI { get; set; }
        public double TurnoverCOI { get; set; }
        public double TurnoverNonCOI { get; set; }
    }
}
