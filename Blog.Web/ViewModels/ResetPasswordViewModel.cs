using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "{0} مورد نیاز است.")]
        [EmailAddress(ErrorMessage = "{0} نامعتبر است.")]
        [Display(Name = "آدرس ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} مورد نیاز است.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "{0} حداقل باید {1} حرف باشد.")]
        [Display(Name = "کلمه عبور جدید")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "{0} مورد نیاز است.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "{0} با {1} مطابقت ندارد.")]
        [Display(Name = "تکرار کلمه عبور جدید")]
        public string ConfirmNewPassword { get; set; }

        public string Token { get; set; }
        public string ReCaptchaToken { get; set; }
    }
}
