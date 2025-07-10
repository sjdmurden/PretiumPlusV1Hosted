namespace CSV_reader.ViewModels
{
    public class ProfileViewModel
    {
        public string UserEmail { get; set; } = string.Empty;

        public List<QuoteInfo> Quotes { get; set; } = new List<QuoteInfo>();

        public class QuoteInfo
        {           
            public string QuoteId { get; set; } = string.Empty;
            public string ClientName { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
        }
    }
}
