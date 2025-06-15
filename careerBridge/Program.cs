using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using careerBridge.Areas.Identity.Data;
using careerBridge.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("careerBridgeDbConnection") ?? throw new InvalidOperationException("Connection string 'careerBridgeDbConnection' not found.");

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
});

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddDbContext<careerBridgeDb>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<careerBridgeUser, IdentityRole>(options => 
options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<careerBridgeDb>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

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


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
