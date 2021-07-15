using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Areas.Admin.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUser ApplicationUser { get; set; }
        public string UserRoles { get; set; }
        public DetailsModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int userId)
        {
            ApplicationUser = await _userManager.FindByIdAsync(userId.ToString());
            if (ApplicationUser is null)
            {
                return NotFound();
            }
            var roles = await _userManager.GetRolesAsync(ApplicationUser) ?? Array.Empty<string>();
            UserRoles = string.Join(", ", roles);

            return Page();
        }
    }
}
