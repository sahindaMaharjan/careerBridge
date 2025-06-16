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

<<<<<<< HEAD
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool DisplayConfirmAccountLink { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string EmailConfirmationUrl { get; set; }

        public void OnGet()
        {
            ViewData["HideNavbar"] = true;
        }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            ViewData["HideNavbar"] = true;

            if (email == null)
            {
                return RedirectToPage("/Index");
            }
            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            DisplayConfirmAccountLink = true;
            if (DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
            }

            return Page();
=======
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound($"Unable to load user with email '{email}'.");
>>>>>>> d9b234b3e6908a25952dcd39233639d4cd1e5ebf
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
