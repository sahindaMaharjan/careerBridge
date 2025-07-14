using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using careerBridge.Areas.Identity.Data;
using careerBridge.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using careerBridge.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) Configure DB connection
var connectionString = builder.Configuration.GetConnectionString("careerBridgeDbConnection")
    ?? throw new InvalidOperationException("Connection string 'careerBridgeDbConnection' not found.");

builder.Services.AddDbContext<careerBridgeDb>(options =>
    options.UseSqlServer(connectionString));

// 2) Add Identity with Roles
builder.Services.AddIdentity<careerBridgeUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<careerBridgeDb>()
    .AddDefaultTokenProviders();

// 3) Configure Identity options (optional)
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
});

// 4) Email service
builder.Services.AddTransient<IEmailSender, EmailSender>();

// 5) Custom job search service
builder.Services.AddScoped<JobSearchService>();

// 6) MVC & Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 7) Seed Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = new[] { "Student", "Employer", "Mentor" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// 8) Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enables default static file handling

// ✅ Custom file serving for resume files stored under wwwroot/resumes
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "resumes")),
    RequestPath = "/resumes"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 9) Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
