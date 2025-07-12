using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using careerBridge.Areas.Identity.Data;
using careerBridge.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) Configure connection string
var connectionString = builder.Configuration
    .GetConnectionString("careerBridgeDbConnection")
    ?? throw new InvalidOperationException("Connection string 'careerBridgeDbConnection' not found.");

// 2) Add DbContext (EF Core)
builder.Services.AddDbContext<careerBridgeDb>(options =>
    options.UseSqlServer(connectionString));

// 3) Configure Identity options (lockout, 2FA, email confirmation, etc.)
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
});

// 4) Add Identity (with roles) and EF stores
builder.Services.AddIdentity<careerBridgeUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<careerBridgeDb>()
    .AddDefaultTokenProviders();

// 5) Email sender for confirmation links
builder.Services.AddTransient<IEmailSender, EmailSender>();

// 6) Add MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 7) Seed initial roles (Student, Employer, Mentor)
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

// 8) HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 9) Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 10) Map routes

// 10a) MVC controllers + views
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 10b) Razor Pages
app.MapRazorPages();

app.Run();
