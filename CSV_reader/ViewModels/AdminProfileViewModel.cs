namespace CSV_reader.ViewModels
{
    public class AdminProfileViewModel
    {
        public string UserEmail { get; set; } = string.Empty;
        public List<AdminQuoteInfo> AllUsersQuotes { get; set; } = new List<AdminQuoteInfo>();

        public class AdminQuoteInfo
        {
            public string UserEmail { get; set; } = string.Empty;
            public string QuoteId { get; set; } = string.Empty;
            public string ClientName { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
        }

        public List<AllUsers> AllUsersInfo { get; set; } = new List<AllUsers>();

        public class AllUsers
        {
            public string UserEmail { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
        }
    }
}
