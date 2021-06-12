using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "لطفا {0} خود را وارد نمائید.")]
        [MinLength(3, ErrorMessage = "{0} حداقل باید {1} حرف باشد.")]
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Required(ErrorMessage = "لطفا {0} خود را وارد نمائید.")]
        [EmailAddress(ErrorMessage = "{0} نامعتبر است.")]
        [Display(Name = "آدرس ایمیل")]
        public string Email { get; set; }

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
        public string ReCaptchaToken { get; set; }
    }
}
