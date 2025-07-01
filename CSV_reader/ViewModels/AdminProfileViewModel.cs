namespace CSV_reader.ViewModels
{
    public class AdminProfileViewModel
    {
        public string UserEmail { get; set; }
        public List<AdminQuoteInfo> AllUsersQuotes { get; set; }

        public class AdminQuoteInfo
        {
            public string UserEmail { get; set; }
            public string QuoteId { get; set; }
            public string ClientName { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public List<AllUsers> AllUsersInfo { get; set; }

        public class AllUsers
        {
            public string UserEmail { get; set; }
            public DateTime CreatedDate { get; set; }
        }
    }
}
