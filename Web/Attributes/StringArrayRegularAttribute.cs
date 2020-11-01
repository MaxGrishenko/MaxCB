using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Web.Attributes
{
    public class StringArrayRegularAttribute : ValidationAttribute
    {
        readonly Regex _rgx;

        public StringArrayRegularAttribute(string pattern)
        {
            _rgx = new Regex(pattern);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] array = value as string[];
            foreach (var item in array)
            {
                if (item == null) { continue; }
                if (!_rgx.IsMatch(item))
                {
                    return new ValidationResult(this.ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
