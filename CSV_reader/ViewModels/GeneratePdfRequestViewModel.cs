namespace CSV_reader.ViewModels
{
    public class GeneratePdfRequestViewModel
    {
        public string BatchId { get; set; } = string.Empty;
        public decimal ClaimsAmount { get; set; }
        public decimal LargeLossFund { get; set; }
        public decimal ReinsuranceCosts { get; set; }
        public decimal ClaimsHandlingFee { get; set; }
        public decimal Levies { get; set; }
        public decimal Expenses { get; set; }
        public decimal Profit { get; set; }
        public decimal NetPremium { get; set; }
        public decimal Commissions { get; set; }
        public decimal GrossPremium { get; set; }
        public decimal UpdatedGrossPremiumPlusIPT { get; set; }
        public string AdjustmentNotes { get; set; } = string.Empty;

        public int FCDaysCOI { get; set; }
        public int FCDaysNonCOI { get; set; }
        public decimal FCTurnoverCOI { get; set; }
        public decimal FCTurnoverNonCOI { get; set; }
    }
}
