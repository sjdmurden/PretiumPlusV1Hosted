using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CSV_reader.Models
{
    public class InputModel
    {
        //public string QuoteNumber { get; set; }
        [Required(ErrorMessage = "Client Name is required.")]
        public string SelectedCoverType { get; set; } = "Self Drive Hire";
        public List<SelectListItem> CoverTypes { get; set; } = new List<SelectListItem>();
        public double Excess { get; set; } = 1000.0;

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        // large loss loading
        public double CarLLL { get; set; } = 40.0;
        public double VanLLL { get; set; } = 50.0;
        public double MinibusLLL { get; set; } = 120.0;
        public double HGVLLL { get; set; } = 120.0;
        /*
        // exposure ratings
        public double CarExp { get; set; }
        public double VanExp { get; set; }
        public double MinibusExp { get; set; }
        public double HGVExp { get; set; }*/
        //public double ExpPercentage { get; set; }

        public string SelectedPercentage { get; set; } = string.Empty;

        //public string ClientName { get; set; }
        //public List<SelectListItem> ClientNames { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Percentages { get; set; } = new List<SelectListItem>();
    }
}
