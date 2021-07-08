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
            var avatarNameClaim = claimsPrincipal.Claims
                .FirstOrDefault(c => c.Type == ApplicationClaimTypes.Avatar);

            var avatarName = avatarNameClaim?.Value;

            return avatarName;
        }
    }
}
