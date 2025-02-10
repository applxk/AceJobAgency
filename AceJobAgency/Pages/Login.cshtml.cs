using System;
using AceJobAgency.Model;
using AceJobAgency.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AceJobAgency.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IEmailSender emailSender;
        public static string GeneratedOTP;

        public LoginModel(SignInManager<IdentityUser> signInManager, IEmailSender emailSender)
        {
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }

        public void OnGet()
        {
            var rememberMeEmail = Request.Cookies["RememberMeEmail"];
            if (!string.IsNullOrEmpty(rememberMeEmail))
            {
                LModel = new Login { Email = rememberMeEmail, RememberMe = true };
            }
            else
            {
                LModel = new Login();
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // reCAPTCHA validation
                var recaptchaResponse = Request.Form["recaptchaResponse"];
                var recaptchaSecret = "6LeartAqAAAAAFBPui9jm6aAaPJREgj0fSDbgXvj"; // Your secret key
                var recaptchaVerificationUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={recaptchaSecret}&response={recaptchaResponse}";

                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(recaptchaVerificationUrl, null);
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var recaptchaResult = JsonConvert.DeserializeObject<ReCaptchaResponse>(jsonResponse);

                    if (!recaptchaResult.Success || recaptchaResult.Score < 0.5) // Adjust score threshold
                    {
                        ModelState.AddModelError("", "reCAPTCHA verification failed.");
                        return Page();
                    }
                }

                var user = await signInManager.UserManager.FindByEmailAsync(LModel.Email);
                if (user != null)
                {
                    // Check if the user is locked out
                    if (await signInManager.UserManager.IsLockedOutAsync(user))
                    {
                        ModelState.AddModelError("", "Your account is locked out. Please try again later.");
                        return Page();
                    }

                    var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, true);
                    if (identityResult.Succeeded)
                    {
                        // Generate a 6-digit OTP
                        GeneratedOTP = new Random().Next(100000, 999999).ToString();

                        // Send OTP to user's email
                        await emailSender.SendEmailAsync(LModel.Email, "Your OTP Code", $"Your OTP code is: {GeneratedOTP}");

                        // Redirect to OTP verification page
                        return RedirectToPage("VerifyOTP", new { email = LModel.Email });

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim("Department", "HR")
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                        // Store email in a cookie if "Remember Me" is checked
                        if (LModel.RememberMe)
                        {
                            Response.Cookies.Append("RememberMeEmail", LModel.Email, new CookieOptions
                            {
                                Expires = DateTime.UtcNow.AddDays(30),
                                HttpOnly = true,
                                Secure = true,
                                IsEssential = true
                            });
                        }
                        else
                        {
                            Response.Cookies.Delete("RememberMeEmail"); // Remove the cookie if not checked
                        }

                        return RedirectToPage("Index");
                    }
                    else if (identityResult.IsLockedOut)
                    {
                        ModelState.AddModelError("", "Your account is locked out. Please try again later.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username or Password incorrect");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Username or Password incorrect");
                }
            }
            return Page();
        }
    }
}