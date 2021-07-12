using Blog.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Identity
{
    public class ApplicationUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
                : base(userManager, roleManager, optionsAccessor)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;

            if (string.IsNullOrWhiteSpace(user.AvatarName) is false)
            {
                identity.AddClaim(new Claim(ApplicationClaimTypes.Avatar, user.AvatarName));
            }

            return principal;
        }

        //protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        //{
        //    var identity = await base.GenerateClaimsAsync(user);

        //    if (string.IsNullOrWhiteSpace(user.AvatarName) is false)
        //    {
        //        identity.AddClaim(new Claim(ApplicationClaimTypes.Avatar, user.AvatarName));
        //    }

        //    return identity;
        //}
    }
}
