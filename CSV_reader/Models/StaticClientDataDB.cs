namespace CSV_reader.Models
{
    public class StaticClientDataDB
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string BatchId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        
        

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
