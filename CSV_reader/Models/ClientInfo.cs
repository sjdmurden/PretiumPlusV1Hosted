using CsvHelper.Configuration.Attributes;

namespace CSV_reader.Models
{
    public class ClientInfo
    {
        public int ID { get; set; }

        public string ClientName { get; set; } = string.Empty;
        public string ClientAddress { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string VATNumber { get; set; } = string.Empty;
    }
}


