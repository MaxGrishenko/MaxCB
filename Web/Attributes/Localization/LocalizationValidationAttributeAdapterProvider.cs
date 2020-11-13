using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Attributes.Localization
{
    public class LocalizationValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            if (attribute is StringArrayRequiredAttribute)
                return new StringArrayRequiredAttributeAdapter(attribute as StringArrayRequiredAttribute, stringLocalizer);
            else if (attribute is StringArrayLengthAttribute)
                return new StringArrayLengthAttributeAdapter(attribute as StringArrayLengthAttribute, stringLocalizer);
            else if (attribute is StringArrayRegularAttribute)
                return new StringArrayRegularAttributeAdapter(attribute as StringArrayRegularAttribute, stringLocalizer);
            else if (attribute is EmailAddressUniqueAttribute)
                return new EmailAddressUniqueAttributeAdapter(attribute as EmailAddressUniqueAttribute, stringLocalizer);
            else if (attribute is UserNameUniqueAttribute)
                return new UserNameUniqueAttributeAdapter(attribute as UserNameUniqueAttribute, stringLocalizer);
            else
                return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
        }
    }
}
