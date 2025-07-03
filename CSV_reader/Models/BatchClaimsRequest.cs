namespace CSV_reader.Models
{
    public class BatchClaimsRequest
    {
        public string BatchId { get; set; } = string.Empty;
        public string SelectedNumOfMonths { get; set; } = string.Empty;
        public string ProjYears { get; set; } = string.Empty;
        public string ChargeCOIFee { get; set; } = string.Empty;
        public string PricingMetric { get; set; } = string.Empty;
        public string PriceBy { get; set; } = string.Empty; 
        public List<string> SelectedClaims { get; set; } = new List<string>();
    }
}
