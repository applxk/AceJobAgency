using AceJobAgency.Model;
using AceJobAgency.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace AceJobAgency.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<IdentityUser> userManager,
                             SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public void OnGet()
        {
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

                    if (!recaptchaResult.Success || recaptchaResult.Score < 0.5) // You can adjust the score threshold
                    {
                        ModelState.AddModelError("", "reCAPTCHA verification failed.");
                        return Page();
                    }
                }

                // Server-side password complexity check
                if (!RModel.IsPasswordStrong())
                {
                    ModelState.AddModelError("RModel.Password", "Password must be at least 12 characters long, and include a combination of lower-case, upper-case, numbers, and special characters.");
                    return Page();
                }

                // Encrypt NRIC before saving it
                string encryptedNRIC = RModel.EncryptNRIC();

                // Create a new ApplicationUser  instance
                var user = new ApplicationUser
                {
                    UserName = RModel.Email,
                    Email = RModel.Email,
                    FirstName = RModel.FirstName,
                    LastName = RModel.LastName,
                    Gender = RModel.Gender,
                    NRIC = encryptedNRIC,
                    DateOfBirth = RModel.DateOfBirth,
                    WhoAmI = RModel.WhoAmI
                };

                // Create the user in ASP.NET Identity
                var result = await userManager.CreateAsync(user, RModel.Password);

                if (result.Succeeded)
                {
                    // Handle file upload (resume)
                    if (RModel.Resume != null && RModel.Resume.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(RModel.Resume.FileName).ToLower();
                        if (fileExtension != ".docx" && fileExtension != ".pdf")
                        {
                            ModelState.AddModelError("RModel.Resume", "Resume must be a .docx or .pdf file.");
                            return Page();
                        }

                        // Check file size (max 5MB)
                        if (RModel.Resume.Length > 5 * 1024 * 1024) // limit file size to 5MB
                        {
                            ModelState.AddModelError("RModel.Resume", "Resume file is too large. Please upload a file smaller than 5MB.");
                            return Page();
                        }

                        // Define file upload path
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, RModel.Resume.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await RModel.Resume.CopyToAsync(stream);
                        }

                        // Save the resume file path in the user object
                        user.ResumePath = filePath;
                    }

                    // Add other claims or authentication steps as necessary
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToPage("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return Page();
        }
    }
}