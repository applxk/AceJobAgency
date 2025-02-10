using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AceJobAgency.Pages
{
    public class ErrorModel : PageModel
    {
        public string ErrorMessage { get; set; }

        public void OnGet(int? statusCode)
        {
            if (statusCode.HasValue)
            {
                if (statusCode == 404)
                {
                    ErrorMessage = "Page Not Found";
                }
                else if (statusCode == 403)
                {
                    ErrorMessage = "Access Denied";
                }
                else
                {
                    ErrorMessage = "An unexpected error occurred.";
                }
            }
            else
            {
                ErrorMessage = "An unexpected error occurred.";
            }
        }
    }
}
