using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Extensions
{
    public static class ModelStateExtension
    {
        public static void AddModelError(this ModelStateDictionary modelState, IEnumerable<IdentityError> identityErrors)
        {
            foreach (var error in identityErrors)
            {

                string key = error.Code switch
                {
                    nameof(IdentityErrorDescriber.DuplicateEmail) or
                    nameof(IdentityErrorDescriber.InvalidEmail) => "Email",

                    nameof(IdentityErrorDescriber.DuplicateUserName) or
                    nameof(IdentityErrorDescriber.InvalidUserName) => "Username",

                    nameof(IdentityErrorDescriber.PasswordMismatch) or
                    nameof(IdentityErrorDescriber.PasswordRequiresDigit) or
                    nameof(IdentityErrorDescriber.PasswordRequiresLower) or
                    nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric) or
                    nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars) or
                    nameof(IdentityErrorDescriber.PasswordTooShort) or
                    nameof(IdentityErrorDescriber.PasswordRequiresUpper) => "Password",

                    _ => null
                };
                if (key is not null)
                {
                    modelState.AddModelError(key, error.Description);
                }
            }
        }
    }
}
