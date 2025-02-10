using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace AceJobAgency.ViewModels
{
    public class Register
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string NRIC { get; set; } // NRIC will be encrypted before saving

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Password field is required.")]
[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$", 
    ErrorMessage = "Password must be at least 12 characters long, and include a combination of lower-case, upper-case, numbers, and special characters.")]
public string Password { get; set; }


        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Resume")]
        public IFormFile Resume { get; set; }

        [Required]
        public string WhoAmI { get; set; } // Allow special characters (no regex restriction applied)

        // Check password complexity
        public bool IsPasswordStrong()
        {
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$");
            return regex.IsMatch(this.Password);
        }

        public string EncryptNRIC()
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(this.NRIC);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
