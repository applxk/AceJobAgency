using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AceJobAgency.Pages
{
    public class VerifyOTPModel : PageModel
    {
        [BindProperty]
        public string OTP { get; set; }
        public string Email { get; set; }
        public string ErrorMessage { get; set; }

        private readonly IEmailSender emailSender;

        public VerifyOTPModel(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public void OnGet(string email)
        {
            Email = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Email = Request.Form["Email"];

            if (OTP == LoginModel.GeneratedOTP) // Ensure you access the static property correctly
            {
                // Clear the OTP after successful verification
                LoginModel.GeneratedOTP = null;

                // Redirect to the index page
                return RedirectToPage("Index");
            }
            else
            {
                ErrorMessage = "Invalid OTP. Please try again.";
                // Optionally, resend the OTP
                LoginModel.GeneratedOTP = new Random().Next(100000, 999999).ToString(); // Generate a new OTP
                await emailSender.SendEmailAsync(Email, "Your OTP Code", $"Your OTP code is: {LoginModel.GeneratedOTP}");
            }

            return Page();
        }
    }
}