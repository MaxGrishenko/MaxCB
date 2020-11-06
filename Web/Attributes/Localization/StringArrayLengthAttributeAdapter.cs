using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Attributes.Localization
{
    public class StringArrayLengthAttributeAdapter : AttributeAdapterBase<StringArrayLengthAttribute>
    {
        public StringArrayLengthAttributeAdapter(StringArrayLengthAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer) { }
        public override void AddValidation(ClientModelValidationContext context) { }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
        }
    }
}
