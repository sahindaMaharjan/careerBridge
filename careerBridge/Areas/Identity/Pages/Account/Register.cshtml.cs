// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using careerBridge.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using careerBridge.Models;
using careerBridge.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

  

namespace careerBridge.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<careerBridgeUser> _signInManager;
        private readonly UserManager<careerBridgeUser> _userManager;
        private readonly IUserStore<careerBridgeUser> _userStore;
        private readonly IUserEmailStore<careerBridgeUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly careerBridgeDb _db;
        private readonly IWebHostEnvironment _env;

        public RegisterModel(
            UserManager<careerBridgeUser> userManager,
            IUserStore<careerBridgeUser> userStore,
            SignInManager<careerBridgeUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, careerBridgeDb db, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _db = db;
            _env = env;

        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [Display(Name = "Full name")]
            public string Fullname { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 
            [Required]
            [Display(Name = "Phone number")]
            public string Phone { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name ="I am a...")]
            public string Role { get; set; }

            //only for employers
            [Required]
            [Display(Name = "Business Certificate (PDF or image)")]
            public IFormFile BusinessCertificate { get; set; }

            //only for students
            public string CollegeName { get; set; }

            //only for mentors
            [Required]
            [Display(Name = "Experience Certificate (PDF or image)")]
            public IFormFile ExperienceCertificate { get; set; }

            [Required]
            [Display(Name = "Expertise Area")]
            public string ExpertiseArea { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.AddToRoleAsync(user, Input.Role);

                    switch (Input.Role)
                    {
                        case "Student":
                            _db.Students.Add(new StudentProfile
                            {
                                
                                FullName = Input.Fullname,
                                Email = Input.Email,
                                Phone = Input.Phone,
                                CollegeName = Input.CollegeName

                            });
                            break;
                        case "Employer":
                            if (Input.BusinessCertificate != null)
                            {
                                var cer = Guid.NewGuid().ToString() + Path.GetExtension(Input.BusinessCertificate.FileName);
                                _db.Employers.Add(new EmployerProfile
                                {

                                    CompanyName = Input.Fullname,
                                    Email = Input.Email,
                                    Phone = Input.Phone,

                                });
                            }
                            break;

                        case "Mentor":
                        if (Input.ExperienceCertificate != null)
                            {
                                _db.Mentors.Add(new MentorProfile
                                {
                                    FullName = Input.Fullname,
                                    Email = Input.Email,
                                    Phone = Input.Phone,
                                    ExpertiseArea = Input.ExpertiseArea
                                });
                            }
                            break;
                    }

                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private careerBridgeUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<careerBridgeUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(careerBridgeUser)}'. " +
                    $"Ensure that '{nameof(careerBridgeUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<careerBridgeUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<careerBridgeUser>)_userStore;
        }
    }
}
