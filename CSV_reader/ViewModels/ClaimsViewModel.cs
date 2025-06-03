using CSV_reader.Models;
using System.Collections.Generic;
using System;

namespace CSV_reader.ViewModels
{
    public class ClaimsViewModel
    {
        public List<ClaimsModel> ClaimsRecords { get; set; } // For the table
        public Dictionary<string, PolicyYearSummary> PolicyYearSummaries { get; set; }
        public HistoricYearsData HistoricYearsData { get; set; }
        public ClaimsViewModel()
        {
            ClaimsRecords = new List<ClaimsModel>();  // Initialise the list
            PolicyYearSummaries = new Dictionary<string, PolicyYearSummary>();
            HistoricYearsData = new HistoricYearsData();
            
        }
        public InputModel InputModel { get; set; }


        public CalculationsModel CalculationsModel { get; set; } = new CalculationsModel();
    }
}