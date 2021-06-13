using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "{0} حداقل باید {1} حرف باشد.")]
        [Display(Name = "نام کاربری")]
        public string Username { get; init; }

        [Required(ErrorMessage = "لطفا {0} خود را وارد نمائید.")]
        [MinLength(3, ErrorMessage = "{0} حداقل باید {1} حرف باشد.")]
        [Display(Name = "نام مستعار")]
        public string FriendlyName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "آدرس ایمیل")]
        public string Email { get; init; }

        [DataType(DataType.Date)]
        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDay { get; set; }

        [Display(Name = "محل اقامت")]
        public string Location { get; set; }

        [Display(Name = "درباره من")]
        public string Bio { get; set; }
    }
}
