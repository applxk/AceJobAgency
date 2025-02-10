using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace AceJobAgency.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public string Email { get; set; }

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                ModelState.AddModelError(string.Empty, "Email is required.");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Page(
                "/ResetPassword",
                pageHandler: null,
                values: new { token, email = Email },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(Email, "Reset Password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return RedirectToPage("ForgotPasswordConfirmation");
        }
    }
}