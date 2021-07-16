using Blog.Web.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Areas.Admin.ViewModels
{
    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "لطفا {0} خود را وارد نمائید.")]
        [MinLength(3, ErrorMessage = "{0} حداقل باید {1} حرف باشد.")]
        [RegularExpression(@"^[a-zA-Z]([._-](?![._-])|[a-zA-Z0-9]){1,18}[a-zA-Z0-9]$",
            ErrorMessage = "{0} نامعتبر است.")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "لطفا {0} خود را وارد نمائید.")]
        [MinLength(3, ErrorMessage = "{0} حداقل باید {1} حرف باشد.")]
        [Display(Name = "نام مستعار")]
        public string FriendlyName { get; set; }


        [Required(ErrorMessage = "لطفا {0} خود را وارد نمائید.")]
        [EmailAddress(ErrorMessage = "{0} نامعتبر است.")]
        [Display(Name = "آدرس ایمیل")]

        public string Email { get; set; }

        [Display(Name = "تایید ایمیل")]
        public bool EmailConfirmed { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "لطفا {0} خود را وارد نمائید.")]
        [MinLength(6, ErrorMessage = "{0} حداقل باید {1} حرف باشد.")]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "لطفا {0} خود را وارد نمائید.")]
        [Compare(nameof(Password), ErrorMessage = "{0} با {1} مطابقت ندارد.")]
        [Display(Name = "تکرار کلمه عبور")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDay { get; set; }

        [Display(Name = "محل اقامت")]
        public string Location { get; set; }

        [Display(Name = "درباره من")]
        public string Bio { get; set; }

        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        [Display(Name = "تصویر پروفایل")]
        public IFormFile Avatar { get; set; }
    }
}
