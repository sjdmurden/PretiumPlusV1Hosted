namespace CSV_reader.ViewModels
{
    public class DocumentSaveToDBViewModel
    {
        /*public int DocumentId { get; set; }
        public DateTime CreatedDate { get; set; }*/
        public string DocumentType { get; set; } = string.Empty;
        public string QuoteNumber { get; set; } = string.Empty;
        public int PolicyNumber { get; set; }
        public string BatchId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}
