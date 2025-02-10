using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AceJobAgency.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly SignInManager<IdentityUser> signInManager;
		public LogoutModel(SignInManager<IdentityUser> signInManager)
		{
			this.signInManager = signInManager;
		}

        public async Task<IActionResult> OnGet()
        {
            // Automatically sign out the user
            await signInManager.SignOutAsync();

            return RedirectToPage("/Login"); // Redirect to the login page
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await signInManager.SignOutAsync();
            return RedirectToPage("Login");
        }

        public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
		}
	}
}
