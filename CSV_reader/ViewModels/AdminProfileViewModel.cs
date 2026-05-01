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
            public bool CanBeDeleted =>
                !string.Equals(UserEmail, "admin@gmail.com", StringComparison.OrdinalIgnoreCase);
            // this is a readon-only boolean prop returning true or false
            // so this chekcs the user emails and if an email is the admin email (ignoring upper/lower case) it sets the CanBeDeleted prop to false so you can't delete the admin user
        }
    }
}
