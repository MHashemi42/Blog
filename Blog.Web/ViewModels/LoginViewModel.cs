using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class LoginViewModel
    {
        
        public string ReturnUrl { get; set; }

        [Required(ErrorMessage = "لطفا {0} خود را وارد نمائید.")]
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "لطفا {0} خود را وارد نمائید.")]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
        public string ReCaptchaToken { get; set; }
    }
}
