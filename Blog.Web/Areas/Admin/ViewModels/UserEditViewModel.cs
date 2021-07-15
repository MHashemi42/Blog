using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Areas.Admin.ViewModels
{
    public class UserEditViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [Display(Name = "تایید ایمیل")]
        public bool EmailConfirmed { get; set; }
        [Display(Name = "امکان مسدود کردن حساب")]
        public bool LockoutEnabled { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "مدت زمان مسدود کردن حساب")]
        public DateTime LockoutEnd { get; set; }
        public string AvatarName { get; set; }
        [Display(Name = "نقش ها")]
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
