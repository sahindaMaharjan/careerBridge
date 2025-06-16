using careerBridge.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Encodings.Web;

public class RegisterConfirmationModel : PageModel
{
    private readonly UserManager<careerBridgeUser> _userManager;

    public RegisterConfirmationModel(UserManager<careerBridgeUser> userManager)
    {
        _userManager = userManager;
    }

    public string Email { get; set; }
    public string EmailConfirmationUrl { get; set; }
    public bool DisplayConfirmAccountLink { get; set; }

    public async Task<IActionResult> OnGetAsync(string email)
    {
        if (email == null)
        {
            return RedirectToPage("/Index");
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound($"Unable to load user with email '{email}'.");
        }

        Email = email;
        DisplayConfirmAccountLink = true; // Change to false in production

        if (DisplayConfirmAccountLink)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = user.Id, code },
                protocol: Request.Scheme);

            EmailConfirmationUrl = HtmlEncoder.Default.Encode(callbackUrl);
        }

        return Page();
    }
}
