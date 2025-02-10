using AceJobAgency.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AceJobAgency.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
                var sessionId = context.User.FindFirst("SessionId")?.Value;

                var user = await userManager.FindByEmailAsync(email);
                if (user != null && user.CurrentSessionId != sessionId)
                {
                    // Invalidate the current session
                    await context.SignOutAsync("MyCookieAuth");
                    context.Response.Redirect("/Login");
                    return;
                }
            }

            await _next(context);
        }
    }
}