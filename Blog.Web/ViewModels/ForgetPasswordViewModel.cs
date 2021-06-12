using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "{0} مورد نیاز است.")]
        [EmailAddress(ErrorMessage = "{0} نامعتبر است.")]
        [Display(Name = "آدرس ایمیل")]
        public string Email { get; set; }

        public string ReCaptchaToken { get; set; }
    }
}
