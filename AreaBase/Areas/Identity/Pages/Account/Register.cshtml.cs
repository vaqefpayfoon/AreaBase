﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AreaBase.Models;
using AreaBase.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace AreaBase.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        //public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "ایمیل")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "حداقل شش کارکتر برای کلمه عبور وارد نمایید", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "کلمه عبور")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "تایید کلمه عبور")]
            [Compare("Password", ErrorMessage = "کلمه عبور و تایید کلمه عبور یکسان نیست")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "نام")]
            public string Name { get; set; }
            [Display(Name = "آدرس")]
            public string StreetAddress { get; set; }
            [Display(Name = "تلفن")]
            public string PhoneNumber { get; set; }
            [Display(Name = "شهر")]
            public string City { get; set; }
            [Display(Name = "استان")]
            public string State { get; set; }
            [Display(Name = "کد پستی")]
            public string PostalCode { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            string role = Request.Form["rdUserRole"].ToString();

            returnUrl = returnUrl ?? Url.Content("~/");
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    City = Input.City,
                    StreetAddress = Input.StreetAddress,
                    State = Input.State,
                    PostalCode = Input.PostalCode,
                    PhoneNumber = Input.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    if (!await _roleManager.RoleExistsAsync(SD.ManagerUser))
                        await _roleManager.CreateAsync(new IdentityRole(SD.ManagerUser));

                    if (!await _roleManager.RoleExistsAsync(SD.CustomerEndUser))
                        await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));

                    if (!await _roleManager.RoleExistsAsync(SD.KitchenUser))
                        await _roleManager.CreateAsync(new IdentityRole(SD.KitchenUser));

                    if (!await _roleManager.RoleExistsAsync(SD.FrontDeskUser))
                        await _roleManager.CreateAsync(new IdentityRole(SD.FrontDeskUser));

                    if (role == SD.KitchenUser)
                    {
                        await _userManager.AddToRoleAsync(user, SD.KitchenUser);
                    }
                    else
                    {
                        if (role == SD.FrontDeskUser)
                        {
                            await _userManager.AddToRoleAsync(user, SD.FrontDeskUser);
                        }
                        else
                        {
                            if (role == SD.ManagerUser)
                            {
                                await _userManager.AddToRoleAsync(user, SD.ManagerUser);
                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(user, SD.CustomerEndUser);
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                return LocalRedirect(returnUrl);
                            }
                        }
                    }

                    return RedirectToAction("Index", "User", new { area = "Admin" });

                    //_logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
