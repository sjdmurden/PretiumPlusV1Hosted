using CSV_reader.Models;

namespace CSV_reader.ViewModels
{
    public class CalculationsViewModel
    {
        public CalculationsModel CalculationsModel { get; set; }
        public InputModel InputModel { get; set; }



        public List<ClaimsModel> ClaimsRecords { get; set; }
        public CalculationsViewModel()
        {
            ClaimsRecords = new List<ClaimsModel>();  // Initialise the list

        }
    }
}
