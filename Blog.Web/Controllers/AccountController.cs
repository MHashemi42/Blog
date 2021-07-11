using Blog.Data.Entities;
using Blog.Data.Identity;
using Blog.Web.Helpers;
using Blog.Web.Services;
using Blog.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ICaptchaService _captchaService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            ICaptchaService captchaService,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _captchaService = captchaService;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("/User/{username}")]
        public async Task<IActionResult> Profile(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
            {
                return NotFound();
            }

            ProfileViewModel viewModel = new()
            {
                Username = user.UserName,
                FriendlyName = user.FriendlyName,
                Bio = user.Bio,
                AvatarPath = Path.Combine("~/images", "users", user.AvatarName ?? "default.jpg")
            };

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
            {
                return NotFound();
            }

            EditProfileViewModel viewModel = new()
            {
                Bio = user.Bio,
                BirthDay = user.BirthDay,
                FriendlyName = user.FriendlyName,
                Location = user.Location,
                Username = user.UserName,
                Email = user.Email,
                AvatarPath = Path.Combine("~/images", "users", user.AvatarName ?? "default.jpg")
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel profileViewModel)
        {
            if (ModelState.IsValid is false)
            {
                return View(profileViewModel);
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
            {
                return NotFound();
            }

            //Upload new avatar
            if (profileViewModel.Avatar is object)
            {
                var ext = Path.GetExtension(profileViewModel.Avatar.FileName);
                user.AvatarName ??= Guid.NewGuid().ToString() + ext;

                var directoryPath = 
                    Path.Combine(_webHostEnvironment.WebRootPath,"images", "users");
                if (Directory.Exists(directoryPath) is false)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var filePath = Path.Combine(directoryPath, user.AvatarName);
                using Image image = Image.Load(profileViewModel.Avatar.OpenReadStream());
                image.Mutate(x => x.Resize(300, 300));
                await image.SaveAsync(filePath);

                //Add avatar name to claims if not exists
                if (User.Claims.Any(c => c.Type == ApplicationClaimTypes.Avatar) is false)
                {
                    var identity = (ClaimsIdentity)User.Identity;
                    var avatarClaim = new Claim(ApplicationClaimTypes.Avatar, user.AvatarName);
                    identity.AddClaim(avatarClaim);
                    await _signInManager.RefreshSignInAsync(user);
                }
            }

            user.FriendlyName = profileViewModel.FriendlyName;
            user.BirthDay = profileViewModel.BirthDay;
            user.Location = profileViewModel.Location;
            user.Bio = profileViewModel.Bio;

            await _userManager.UpdateAsync(user);

            profileViewModel.AvatarPath =
                    Path.Combine("~/images", "users", user.AvatarName ?? "default.jpg");

            TempData["Message"] = "پروفایل با موفقیت ویرایش شد.";

            return RedirectToAction(nameof(Profile), new { username = user.UserName });
        }

        [Authorize]
        public async Task<IActionResult> DeleteAvatar()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(user.AvatarName))
            {
                return BadRequest();
            }

            var directoryPath =
                    Path.Combine(_webHostEnvironment.WebRootPath, "images", "users");
            var avatarPath = Path.Combine(directoryPath, user.AvatarName);
            System.IO.File.Delete(avatarPath);

            user.AvatarName = null;
            await _userManager.UpdateAsync(user);

            var avatarNameClaim = User.Claims.First(c => c.Type == ApplicationClaimTypes.Avatar);
            var identity = (ClaimsIdentity)User.Identity;
            identity.RemoveClaim(avatarNameClaim);
            await _signInManager.RefreshSignInAsync(user);

            return Ok();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid is false)
            {
                return View();
            }

            var isReCaptchaValid = await _captchaService
                .VerifyReCaptcha(registerViewModel.ReCaptchaToken);

            if (isReCaptchaValid is false)
            {
                ModelState.AddModelError(string.Empty, "مشکلی رخ داده است. لطفا مجدد تلاش کنید.");
                return View();
            }

            ApplicationUser newUser = new()
            {
                UserName = registerViewModel.Username,
                FriendlyName = registerViewModel.FriendlyName,
                Email = registerViewModel.Email
            };

            var createResult = await _userManager.CreateAsync(newUser, registerViewModel.Password);
            if (createResult.Succeeded)
            {
                await SendConfirmEmail(newUser);
                return RedirectToAction(nameof(ConfirmEmailMessage));
            }

            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        public IActionResult Login(string returnUrl = "/")
            => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid is false)
            {
                return View();
            }

            var isReCaptchaValid = await _captchaService
                .VerifyReCaptcha(loginViewModel.ReCaptchaToken);

            if (isReCaptchaValid is false)
            {
                ModelState.AddModelError(string.Empty, "مشکلی رخ داده است. لطفا مجدد تلاش کنید.");
                return View();
            }

            var user = await _userManager.FindByNameAsync(loginViewModel.Username);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "نام کاربری یا رمز عبور اشتباه است.");
                return View();
            }

            var signInResult = await _signInManager
                .PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: false);

            if (signInResult.IsNotAllowed)
            {
                await SendConfirmEmail(user);
                return RedirectToAction(nameof(ConfirmEmailMessage));
            }

            return LocalRedirect(loginViewModel.ReturnUrl ?? "/");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public IActionResult ConfirmEmailMessage() => View();

        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailViewModel confirmEmailViewModel)
        {
            var user = await _userManager.FindByIdAsync(confirmEmailViewModel.UserId.ToString());
            if (user is null)
            {
                return NotFound();
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmEmailViewModel.Code));
            var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, token);
            if (confirmEmailResult.Succeeded)
            {
                return View();
            }

            return NotFound();
        }

        public IActionResult ResendEmailConfirmation() => View();

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation
            (ResendEmailConfirmationViewModel emailConfirmationViewModel)
        {
            if (ModelState.IsValid is false)
            {
                return View();
            }

            var isReCaptchaValid = await _captchaService
                .VerifyReCaptcha(emailConfirmationViewModel.ReCaptchaToken);

            if (isReCaptchaValid is false)
            {
                ModelState.AddModelError(string.Empty, "مشکلی رخ داده است. لطفا مجدد تلاش کنید.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(emailConfirmationViewModel.Email);
            if (user is null || user.EmailConfirmed)
            {
                return RedirectToAction(nameof(ConfirmEmailMessage));
            }

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            emailConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));

            await SendConfirmEmail(user);

            return RedirectToAction(nameof(ConfirmEmailMessage));
        }

        public IActionResult ForgetPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            if (ModelState.IsValid is false)
            {
                return View();
            }

            var isReCaptchaValid = await _captchaService
                .VerifyReCaptcha(forgetPasswordViewModel.ReCaptchaToken);

            if (isReCaptchaValid is false)
            {
                ModelState.AddModelError(string.Empty, "مشکلی رخ داده است. لطفا مجدد تلاش کنید.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(forgetPasswordViewModel.Email);
            if (user is null)
            {
                return RedirectToAction(nameof(ForgetPasswordConfirmation));
            }

            await SendResetPasswordEmail(user);

            return RedirectToAction(nameof(ForgetPasswordConfirmation));
        }

        public IActionResult ForgetPasswordConfirmation() => View();

        public IActionResult ResetPassword(string code)
        {
            if (code is null)
            {
                return NotFound();
            }

            ResetPasswordViewModel viewModel = new()
            {
                Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid is false)
            {
                return View();
            }

            var isReCaptchaValid = await _captchaService
                .VerifyReCaptcha(resetPasswordViewModel.ReCaptchaToken);

            if (isReCaptchaValid is false)
            {
                ModelState.AddModelError(string.Empty, "مشکلی رخ داده است. لطفا مجدد تلاش کنید.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
            if (user is null)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(
                user, resetPasswordViewModel.Token, resetPasswordViewModel.NewPassword);

            if (resetPasswordResult.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in resetPasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        public IActionResult ResetPasswordConfirmation() => View();

        public IActionResult AccessDenied() => View();

        private async Task SendConfirmEmail(ApplicationUser user)
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            emailConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));

            string url = Url.Action(
                action: nameof(ConfirmEmail),
                controller: "Account",
                values: new { code = emailConfirmationToken, userId = user.Id },
                protocol: Request.Scheme,
                host: Request.Host.ToString());

            _emailService.Send(
                to: user.Email,
                subject: "تایید ایمیل",
                html: $"<h1>تایید ایمیل</h1>\n<p>برای تایید ایمیل <a href=\"{url}\">اینجا</a> را کلیک کنید.</p>");
        }

        private async Task SendResetPasswordEmail(ApplicationUser user)
        {
            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            passwordResetToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(passwordResetToken));

            string url = Url.Action(
                action: nameof(ResetPassword),
                controller: "Account",
                values: new { code = passwordResetToken },
                protocol: Request.Scheme,
                host: Request.Host.ToString());

            _emailService.Send(
                to: user.Email,
                subject: "ویرایش کلمه عبور",
                html: $"<h1>ویرایش کلمه عبور</h1>\n<p>برای ویرایش کلمه عبور <a href=\"{url}\">اینجا</a> را کلیک کنید.</p>");
        }
    }

}
