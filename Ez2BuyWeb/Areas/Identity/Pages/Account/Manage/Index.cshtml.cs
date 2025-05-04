// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ez2Buy.DataAccess.Models; // For AppUser

namespace Ez2BuyWeb.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            public string Name { get; set; }

            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Street Address")]
            public string StreetAddress { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }

            [Display(Name = "Governorate")]
            public string Governorate { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var appUser = user as AppUser; // Cast to AppUser to access custom properties

            Username = userName;

            Input = new InputModel
            {
                Name = appUser?.Name ?? "",
                PhoneNumber = phoneNumber,
                StreetAddress = appUser?.StreetAddress ?? "",
                City = appUser?.City ?? "",
                Governorate = appUser?.Governorate ?? ""
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var appUser = user as AppUser; // Cast to AppUser to update custom properties
            if (appUser == null)
            {
                TempData["error"] = "Unexpected error: User type mismatch.";
                return RedirectToPage();
            }

            // Update Name
            if (Input.Name != appUser.Name)
            {
                appUser.Name = Input.Name;
            }

            // Update PhoneNumber
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    TempData["error"] = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // Update Address Fields
            if (Input.StreetAddress != appUser.StreetAddress)
            {
                appUser.StreetAddress = Input.StreetAddress;
            }

            if (Input.City != appUser.City)
            {
                appUser.City = Input.City;
            }

            if (Input.Governorate != appUser.Governorate)
            {
                appUser.Governorate = Input.Governorate;
            }

            // Save changes to the user
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                TempData["error"] = "Unexpected error when trying to update profile.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["success"] = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}