namespace CSV_reader.Models
{
    public class IndivClaimDB
    {

        public int Id { get; set; }

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


        // the following is static data. This is the same info in every row for each batch of claims hence why I'm calling it static
        // I'm going to try and change the database to have a table of this data separately so the claims table isn't as big
        // this isn't an essential change but more of a challenge to clean up the project
        public int RDaysCOI { get; set; }
        public int RDaysNonCOI { get; set; }
        public double TurnoverCOI { get; set; }
        public double TurnoverNonCOI { get; set; }

        public double ForecastTO_COI { get; set; }
        public double ForecastTO_NonCOI { get; set; }
        public int ForecastDaysCOI { get; set; }
        public int ForecastDaysNonCOI { get; set; }

        public int CarNums { get; set; }
        public int VanNums { get; set; }
        public int MinibusNums { get; set; }
        public int HGVNums { get; set; }

        public string ClientCoverType { get; set; } = string.Empty;
        public double ClientExcess { get; set; }
        public string ClientStartDate { get; set; } = string.Empty;
        public string ClientEndDate { get; set; } = string.Empty;

        public double ClientCarLLL { get; set; }
        public double ClientVanLLL { get; set; }
        public double ClientMBusLLL { get; set; }
        public double ClientHGVLLL { get; set; }

        public double CarExposure { get; set; }
        public double VanExposure { get; set; }
        public double MinibusExposure { get; set; }
        public double HGVExposure { get; set; }
        public double ExpoPercentage { get; set; }


    }
}
