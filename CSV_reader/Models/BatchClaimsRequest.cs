namespace CSV_reader.Models
{
    public class BatchClaimsRequest
    {
        public string BatchId { get; set; }
        public string SelectedNumOfMonths { get; set; }
        public string ProjYears { get; set; }
        public string ChargeCOIFee { get; set; }
        public string PricingMetric { get; set; }
        public string PriceBy { get; set; }
        public List<string> SelectedClaims { get; set; }
    }
}
