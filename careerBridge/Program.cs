using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using careerBridge.Areas.Identity.Data;
using careerBridge.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure connection string
var connectionString = builder.Configuration.GetConnectionString("careerBridgeDbConnection")
    ?? throw new InvalidOperationException("Connection string 'careerBridgeDbConnection' not found.");

// Configure Identity options
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
});

// Add EF DbContext
builder.Services.AddDbContext<careerBridgeDb>(options =>
    options.UseSqlServer(connectionString));

// Add Identity with roles + token providers (for 2FA)
builder.Services.AddIdentity<careerBridgeUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<careerBridgeDb>()
    .AddDefaultTokenProviders(); // ?? Needed for 2FA

// Email sender (for confirmation links)
builder.Services.AddTransient<IEmailSender, EmailSender>();

// MVC and Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Seed Roles
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

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
