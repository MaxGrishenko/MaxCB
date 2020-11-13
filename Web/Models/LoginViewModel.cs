using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class LoginViewModel
    {
        [Required (ErrorMessage = "email_required")]
        [EmailAddress]
        [StringLength(30)]
        [Display(Name = "email_display")]
        public string Email { get; set; }

        [Required (ErrorMessage = "password_required")]
        [DataType(DataType.Password)]
        [StringLength(20)]
        [Display(Name = "password_display")]
        public string Password { get; set; }

        [Display(Name = "remember_display")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        // AuthenticationScheme is in Microsoft.AspNetCore.Authentication namespace
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
