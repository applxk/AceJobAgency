using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace AceJobAgency.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string Email { get; private set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                Email = user.Email;
            }
        }
    }
}
