using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Web.Attributes;

namespace Web.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "email_required")]
        [EmailAddress]
        [StringLength(30)]
        [EmailAddressUnique(ErrorMessage = "email_unique")]
        [Display(Name = "email_display", Prompt = "email_promt")]
        public string Email { get; set; }

        [Required(ErrorMessage = "name_required")]
        [UserNameUnique (ErrorMessage = "name_unique")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "name_length")]
        [RegularExpression("[a-zA-Z0-9]+", ErrorMessage = "name_regular")]
        [Display(Name = "name_display", Prompt = "name_promt")]
        public string Name { get; set; }

        [Required(ErrorMessage = "password_required")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "password_length")]
        [RegularExpression("[a-zA-Z0-9]+", ErrorMessage = "password_regular")]
        [DataType(DataType.Password)]
        [Display(Name = "password_display")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "password_confirm_display")]

        [Compare("Password", ErrorMessage = "password_confirm_compare")]
        public string PasswordConfirm { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
