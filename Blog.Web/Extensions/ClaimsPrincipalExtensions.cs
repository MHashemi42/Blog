using Blog.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetAvatarName(this ClaimsPrincipal claimsPrincipal)
        {
            var avatarName = claimsPrincipal.Claims
                .FirstOrDefault(c => c.Type == ApplicationClaimTypes.Avatar).Value;

            return avatarName;
        }
    }
}
