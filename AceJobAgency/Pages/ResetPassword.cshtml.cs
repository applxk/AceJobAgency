using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace AceJobAgency.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Token { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string ConfirmPassword { get; set; }

        public ResetPasswordModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public void OnGet(string token, string email)
        {
            Token = token;
            Email = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }

            // Server-side password complexity check
            if (!IsPasswordStrong(Password))
            {
                ModelState.AddModelError("Password", "Password must be at least 12 characters long, and include a combination of lower-case, upper-case, numbers, and special characters.");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return RedirectToPage("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Token, Password);
            if (result.Succeeded)
            {
                return RedirectToPage("ResetPasswordConfirmation"); // Redirect to confirmation page
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        private bool IsPasswordStrong(string password)
        {
            // Regular Expression (Matches the server-side rule)
            var regex = new System.Text.RegularExpressions.Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$");
            return regex.IsMatch(password);
        }
    }
}