using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data.Entities;
using Blog.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Areas.Admin.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        [BindProperty]
        public UserEditViewModel UserEdit { get; set; }
        public SelectList SelectListRoles { get; set; }
        public EditModel(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnGetAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return NotFound();
            }

            IList<string> allRoles = await _roleManager.Roles
                .Select(r => r.Name)
                .ToListAsync();
            
            IList<string> userRoles = await _userManager
                .GetRolesAsync(user);

            SelectListRoles = new SelectList(
                items: allRoles, selectedValue: userRoles);
            
            UserEdit = new UserEditViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                AvatarName = user.AvatarName,
                LockoutEnd = user.LockoutEnd?.DateTime ?? DateTime.Now,
                Roles = userRoles
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int userId)
        {
            if (UserEdit is null || UserEdit.Id != userId)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRolesAsync(user, UserEdit.Roles);

            user.EmailConfirmed = UserEdit.EmailConfirmed;
            user.LockoutEnabled = UserEdit.LockoutEnabled;
            user.LockoutEnd = new DateTimeOffset(UserEdit.LockoutEnd);
            await _userManager.UpdateAsync(user);

            return RedirectToPage("Details", new { userId = user.Id });
        }
    }
}
