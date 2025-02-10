using AceJobAgency.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configure lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Lockout duration
    options.Lockout.MaxFailedAccessAttempts = 3; // Max failed attempts
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";

    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization(Options =>
{
    Options.AddPolicy("MustBelongToHRDepartment",
        policy => policy.RequireClaim("Department", "HR"));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login"; // Redirect to login page
    options.ExpireTimeSpan = TimeSpan.FromMinutes(1); // 1-minute session timeout
    options.SlidingExpiration = false; // Prevent extending session on activity
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePagesWithRedirects("errors/{0}");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
