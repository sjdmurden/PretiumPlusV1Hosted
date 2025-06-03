using CSV_reader.Models;
using System.ComponentModel.DataAnnotations;

namespace CSV_reader.ViewModels
{
    public class LoginViewModel
    {
         
        [Required(ErrorMessage = "Required")]
        [MaxLength(100)]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [MaxLength(255)]
        public string UserPassword { get; set; }
    
    }
}
