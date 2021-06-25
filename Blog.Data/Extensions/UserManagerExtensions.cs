﻿using Blog.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Extensions
{
    public static class UserManagerExtensions
    {
        public async static Task<ApplicationUser> GetUserWithAvatar
            (this UserManager<ApplicationUser> userManager, string username)
        {
            var user = await userManager.Users
                        .Include(u => u.Avatar)
                        .SingleOrDefaultAsync(u => u.NormalizedUserName == username.ToUpper());

            return user;
        }
    }
}