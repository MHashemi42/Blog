using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data.Entities;
using Blog.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Blog.Web.Areas.Admin.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public UserCreateViewModel NewUser { get; set; }
        public CreateModel(UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid is false)
            {
                return Page();
            }

            ApplicationUser newUser = new()
            {
                UserName = NewUser.UserName,
                FriendlyName = NewUser.FriendlyName,
                Email = NewUser.Email,
                EmailConfirmed = NewUser.EmailConfirmed,
                BirthDay = NewUser.BirthDay,
                Location = NewUser.Location,
                Bio = NewUser.Bio
            };

            if (NewUser.Avatar is object)
            {
                var directoryPath =
                Path.Combine(_webHostEnvironment.WebRootPath, "images", "users");
                if (Directory.Exists(directoryPath) is false)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var ext = Path.GetExtension(NewUser.Avatar.FileName);
                var avatarName = Guid.NewGuid().ToString() + ext;
                var filePath = Path.Combine(directoryPath, avatarName);

                using Image image = Image.Load(NewUser.Avatar.OpenReadStream());
                image.Mutate(x => x.Resize(300, 300));
                await image.SaveAsync(filePath);

                newUser.AvatarName = avatarName;
            }

            var result = await _userManager.CreateAsync(newUser, NewUser.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
