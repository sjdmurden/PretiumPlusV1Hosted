using CSV_reader.Models;
using System.ComponentModel.DataAnnotations;

namespace CSV_reader.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Required")]
        [MaxLength(100)]
        public string UserEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [MaxLength(255)]
        public string UserPassword { get; set; } = string.Empty;
    }
}
