namespace CSV_reader.ViewModels
{
    public class ProfileViewModel
    {
        public string UserEmail { get; set; }

        public List<QuoteInfo> Quotes { get; set; }

        public class QuoteInfo
        {           
            public string QuoteId { get; set; }
            public string ClientName { get; set; }
            public DateTime CreatedDate { get; set; }
        }
    }
}
