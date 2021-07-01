﻿using Blog.Data.Entities;
using Blog.Data.Extensions;
using Blog.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewComponents
{
    public class UserNavbarViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _memoryCache;

        public UserNavbarViewComponent(
            UserManager<ApplicationUser> userManager,
            IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _memoryCache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_memoryCache.TryGetValue("_Avatar", out string avatarDataUrl) is false &&
                User.Identity.IsAuthenticated)
            {
                //Key is not cache, so get value
                var avatar = await _userManager.GetAvatarByUsername(User.Identity.Name);
                if (avatar is object)
                {
                    string imageBase64Data = Convert.ToBase64String(avatar.ImageData);
                    string imageDataURL = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
                    avatarDataUrl = imageDataURL;
                }
                else
                {
                    avatarDataUrl = DefaultAvatar.DEFAULT;
                }

                //Set cache options
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

                //Save data in cache
                _memoryCache.Set("_Avatar", avatarDataUrl, cacheEntryOptions);
            }

            ViewBag.AvatarDataUrl = avatarDataUrl;

            return View();
        }
    }
}
