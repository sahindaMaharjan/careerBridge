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
        [Required]
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
            [Display(Name = "Business Certificate (PDF or image)")]
            public IFormFile BusinessCertificate { get; set; }

            //only for students
            public string CollegeName { get; set; }

            //only for mentors
            [Display(Name = "Experience Certificate (PDF or image)")]
            public IFormFile ExperienceCertificate { get; set; }

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

            _logger.LogInformation("ModelState Valid: " + ModelState.IsValid);

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogError("Validation error: " + error.ErrorMessage);
                    }
                }
                return Page(); // Redisplay form if invalid
            }

            var user = CreateUser();
            user.Fullname = Input.Fullname;
            user.PhoneNumber = Input.Phone;
            user.RoleType = Input.Role;
            user.CreatedAt = DateTime.UtcNow;
            user.Email = Input.Email;

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var userId = await _userManager.GetUserIdAsync(user);
                await _userManager.AddToRoleAsync(user, Input.Role);

                // Save profile based on role
                switch (Input.Role)
                {
                    case "Student":
                        _logger.LogInformation("Saving Student Profile...");
                        _db.Students.Add(new StudentProfile
                        {
                            UserID = userId,
                            FullName = Input.Fullname,
                            Email = Input.Email,
                            Phone = Input.Phone,
                            CollegeName = Input.CollegeName
                        });
                        break;

                    case "Employer":
                        _logger.LogInformation("Saving Employer Profile...");
                        if (Input.BusinessCertificate != null)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.BusinessCertificate.FileName);
                            var filePath = Path.Combine(_env.WebRootPath, "certificates", fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await Input.BusinessCertificate.CopyToAsync(stream);
                            }
                            _db.Employers.Add(new EmployerProfile
                            {
                                UserID = userId,
                                CompanyName = Input.Fullname,
                                Email = Input.Email,
                                Phone = Input.Phone,
                                BusinessCertificatePath = "/certificates/" + fileName
                            });
                        }
                        break;

                    case "Mentor":
                        _logger.LogInformation("Saving Mentor Profile...");
                        if (Input.ExperienceCertificate != null)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.ExperienceCertificate.FileName);
                            var filePath = Path.Combine(_env.WebRootPath, "certificates", fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await Input.ExperienceCertificate.CopyToAsync(stream);
                            }
                            _db.Mentors.Add(new MentorProfile
                            {
                                UserID = userId,
                                FullName = Input.Fullname,
                                Email = Input.Email,
                                Phone = Input.Phone,
                                ExpertiseArea = Input.ExpertiseArea,
                                CertificatePath = "/certificates/" + fileName
                            });
                        }
                        break;
                }

                await _db.SaveChangesAsync();

                // Email confirmation
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
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
                    return Input.Role switch
                    {
                        "Student" => RedirectToAction("Index", "Student"),
                        "Employer" => RedirectToAction("Index", "Employer"),
                        "Mentor" => RedirectToAction("Index", "Mentor"),
                        _ => RedirectToPage("Index")
                    };
                }
            }

            // If creation failed, show errors
            foreach (var error in result.Errors)
            {
                _logger.LogError("User creation error: " + error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }

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
