using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AceJobAgency.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ChangePasswordModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public ChangePasswordInputModel Input { get; set; }

        public class ChangePasswordInputModel
        {
            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string CurrentPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
            public string ConfirmNewPassword { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Server-side password complexity check
            if (!IsPasswordStrong(Input.NewPassword))
            {
                ModelState.AddModelError("Input.NewPassword", "Password must be at least 12 characters long, and include a combination of lower-case, upper-case, numbers, and special characters.");
                return Page();
            }

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "No account found with this email.");
                return Page();
            }

            // Attempt to change the password
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            TempData["Message"] = "Your password has been changed successfully.";
            return RedirectToPage("/Login");
        }

        private bool IsPasswordStrong(string password)
        {
            // Regular Expression (Matches the server-side rule)
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$");
            return regex.IsMatch(password);
        }
    }
}