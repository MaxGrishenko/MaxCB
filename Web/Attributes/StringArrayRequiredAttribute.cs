using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Attributes
{
    public class StringArrayRequiredAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] array = value as string[];

            if (array == null || array.Any(item => string.IsNullOrEmpty(item)))
            {
                string errorMessage = FormatErrorMessage(ErrorMessageString);
                return new ValidationResult(errorMessage);
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
