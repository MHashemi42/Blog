using Blog.Data.Entities;
using Blog.Data.Helpers;
using Blog.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Extensions
{
    public static class UserManagerExtensions 
    {
        public static async Task<PagedList<ApplicationUser>> GetUsersAsync(
            this UserManager<ApplicationUser> userManager,
            ApplicationUserParameters parameters)
        {
            var source = userManager.Users;

            if (string.IsNullOrWhiteSpace(parameters.Query) is false)
            {
                source = source.Where(u =>
                    (parameters.QueryByUsername && u.UserName.Contains(parameters.Query)) ||
                    (parameters.QueryByEmail && u.Email.Contains(parameters.Query)));
            }
            source = source.OrderBy(u => u.Id);

            var pagedList = await PagedList<ApplicationUser>
                .ToPagedListAsync(source, parameters.PageNumber, parameters.PageSize);

            return pagedList;
        }
    }
}
