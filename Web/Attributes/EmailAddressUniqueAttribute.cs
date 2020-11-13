using Repo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Attributes
{
    public class EmailAddressUniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _context = (ApplicationContext)validationContext.GetService(typeof(ApplicationContext));
            if (value == null) return ValidationResult.Success;
            var entity = _context.Users.SingleOrDefault(e => e.Email == value.ToString());
            if (entity != null)
            {
                return new ValidationResult(this.ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
