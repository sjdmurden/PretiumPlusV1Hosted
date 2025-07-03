namespace CSV_reader.Models
{
    public class IndivClaimDataDB
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserEmail { get; set; } = string.Empty;

        // Unique identifier for each uploaded file - each batch of claims will have this Id
        public string BatchId { get; set; } = string.Empty;

        public string ClientName { get; set; } = string.Empty;
        public string PolicyYear { get; set; } = string.Empty;
        public string ClaimRef { get; set; } = string.Empty;
        public string LossDate { get; set; } = string.Empty;
        public string ReportedDate { get; set; } = string.Empty;
        public string Registration { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string IncidentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

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
