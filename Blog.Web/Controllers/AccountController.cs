using Blog.Data.Entities;
using Blog.Web.Helpers;
using Blog.Web.Services;
using Blog.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
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

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            ICaptchaService captchaService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _captchaService = captchaService;
        }

        [Route("/User/{username}")]
        public async Task<IActionResult> Profile(string username)
        {
            var user = await _userManager.Users
                        .Include(u => u.Avatar)
                        .SingleOrDefaultAsync(u => u.NormalizedUserName == username.ToUpper());

            if (user is null)
            {
                return NotFound();
            }

            ProfileViewModel viewModel = new()
            {
                Username = user.UserName,
                FriendlyName = user.FriendlyName,
                Bio = user.Bio
            };

            if (user.Avatar is object)
            {
                string imageBase64Data = Convert.ToBase64String(user.Avatar.ImageData);
                string imageDataURL = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
                viewModel.AvatarDataUrl = imageDataURL;
            }
            else
            {
                viewModel.AvatarDataUrl = DefaultAvatar.DEFAULT;
            }

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.Users
                        .Include(u => u.Avatar)
                        .SingleOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpper());

            EditProfileViewModel viewModel = new()
            {
                Bio = user.Bio,
                BirthDay = user.BirthDay,
                FriendlyName = user.FriendlyName,
                Location = user.Location,
                Username = user.UserName,
                Email = user.Email
            };

            if (user.Avatar is object)
            {
                string imageBase64Data = Convert.ToBase64String(user.Avatar.ImageData);
                string imageDataURL = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
                viewModel.AvatarDataUrl = imageDataURL;
            }
            else
            {
                viewModel.AvatarDataUrl = DefaultAvatar.DEFAULT;
            }

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
            
            var user = await _userManager.Users
                        .Include(u => u.Avatar)
                        .SingleOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpper());

            user.FriendlyName = profileViewModel.FriendlyName;
            user.BirthDay = profileViewModel.BirthDay;
            user.Location = profileViewModel.Location;
            user.Bio = profileViewModel.Bio;

            if (profileViewModel.Avatar is object)
            {
                user.Avatar ??= new Avatar();
                user.Avatar.ImageTitle = profileViewModel.Avatar.FileName;

                using var ms = new MemoryStream();
                await profileViewModel.Avatar.CopyToAsync(ms);
                user.Avatar.ImageData = ms.ToArray();
            }

            await _userManager.UpdateAsync(user);

            if (user.Avatar is object)
            {
                string imageBase64Data = Convert.ToBase64String(user.Avatar.ImageData);
                string imageDataURL = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
                profileViewModel.AvatarDataUrl = imageDataURL;
            }
            else
            {
                profileViewModel.AvatarDataUrl = DefaultAvatar.DEFAULT;
            }

            return View(profileViewModel);
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
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                emailConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));

                string url = Url.Action(
                    action: nameof(ConfirmEmail),
                    controller: "Account",
                    values: new { code = emailConfirmationToken, userId = newUser.Id },
                    protocol: Request.Scheme,
                    host: Request.Host.ToString());

                _emailService.Send(
                    to: newUser.Email,
                    subject: "تایید ایمیل",
                    html: $"<h1>تایید ایمیل</h1>\n<p>برای تایید ایمیل <a href=\"{url}\">اینجا</a> را کلیک کنید.</p>");

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
                ModelState.AddModelError(string.Empty, "آدرس ایمیل شما تایید نشده است.");
                return View();
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
            var isEmailConfirm = await _userManager.IsEmailConfirmedAsync(user);
            if (user is null || isEmailConfirm is false)
            {
                return RedirectToAction(nameof(ForgetPasswordConfirmation));
            }

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
    }

}
