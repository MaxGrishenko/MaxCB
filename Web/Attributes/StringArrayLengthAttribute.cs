using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Attributes
{
    public class StringArrayLengthAttribute : ValidationAttribute
    {
        readonly int _minValue;
        readonly int _maxValue;

        public StringArrayLengthAttribute(int minValue, int maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] array = value as string[];
            foreach (var item in array)
            {
                if (item == null) { continue; }
                if (item.Length < _minValue || item.Length > _maxValue) {
                    return new ValidationResult(this.ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
