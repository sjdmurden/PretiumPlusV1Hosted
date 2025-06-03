/*using System.ComponentModel.DataAnnotations;

namespace CSV_reader.Models
{
    public class LoginModel
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserLogin { get; set; }  // unique login name

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }  // display name

        [Required]
        [MaxLength(255)]
        public string UserPassword { get; set; }
        public int UserType { get; set; } // 1 = admin, 2 = user
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
*/