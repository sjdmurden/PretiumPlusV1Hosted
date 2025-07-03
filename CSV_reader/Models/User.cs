using System.ComponentModel.DataAnnotations;

namespace CSV_reader.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserEmail { get; set; } = string.Empty;  // unique login name

        /*[Required]
        [MaxLength(100)]
        public string UserName { get; set; }  // display name*/

        [Required]
        [MaxLength(255)]
        public string UserPassword { get; set; } = string.Empty;
        public int UserType { get; set; } // 1 = admin, 2 = user
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        /*public bool IsEmailVerified { get; set; }  // Track verification status
        public string EmailVerificationToken { get; set; } // Store token for verification*/
    }
}
