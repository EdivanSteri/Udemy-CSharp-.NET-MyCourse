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
using MyCourse.Models.Entities;

namespace MyCourse.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public DateTimeOffset EcommerceConsent { get; set; }
        public DateTimeOffset? NewsletterConsent { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Il nome completo è obbligatorio")]
            [StringLength(100, MinimumLength = 3, ErrorMessage = "Il nome completo deve essere di almeno {2} e di al massimo {1} caratteri.")]
            [Display(Name = "Nome completo")]
            public string FullName { get; set; }

            [Phone(ErrorMessage = "Deve essere un numero di telefono valido")]
            [Display(Name = "Numero di telefono")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Iscrizione alla newsletter")]
            public bool NewsletterConsent { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            EcommerceConsent = user.EcommerceConsent;
            NewsletterConsent = user.NewsletterConsent;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                NewsletterConsent = NewsletterConsent is not null,
                FullName = user.FullName
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

            //TODO: PERSISTERE IL FULLNAME
            if (Input.FullName != null)
            {
                //Passo1: Recuperare l'istanza di ApplicationUser (in realtà è stato fatto alla riga 94)
                //Passo2: Modificare la sua proprietà FullName ottenendo il valore dall'input model
                user.FullName = Input.FullName;
                //Passo3: Persistere l'ApplicationUser invocando il metodo UpdateAsync dello user manager 
                IdentityResult resul = await _userManager.UpdateAsync(user);
                //Passo4: Consultare la proprietà Success dell'IdentityResult perché se è false, visualizza un errore
                if (!resul.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set Full Name.";
                    return RedirectToPage();
                }
            }

            if (Input.NewsletterConsent)
            {
                if (user.NewsletterConsent is null)
                {
                    user.NewsletterConsent = DateTimeOffset.Now;
                    IdentityResult resul = await _userManager.UpdateAsync(user);
                    if (!resul.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set Newsletter Acconsent.";
                        return RedirectToPage();
                    }
                }
            }
            else
            {
                user.NewsletterConsent = null;
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
