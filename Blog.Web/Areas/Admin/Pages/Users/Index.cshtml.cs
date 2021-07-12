using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data.Entities;
using Blog.Data.Extensions;
using Blog.Data.Helpers;
using Blog.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Areas.Admin.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PagedList<ApplicationUser> Users { get; set; }
        public string PreviousDisabled { get; private set; }
        public string NextDisabled { get; private set; }

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet([FromQuery] ApplicationUserParameters parameters)
        {
            if (parameters.PageNumber < 1)
            {
                return RedirectToPage("Index", new { pageNumber = 1 });
            }

            Users = await _userManager.GetUsersAsync(parameters);
            PreviousDisabled = Users.HasPrevious ? string.Empty : "disabled";
            NextDisabled = Users.HasNext ? string.Empty : "disabled";

            return Page();
        }
    }
}
