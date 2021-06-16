using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Display(Name = "نام مستعار")]
        public string FriendlyName { get; set; }

        [Display(Name = "درباره من")]
        public string Bio { get; set; }

        [DataType(DataType.ImageUrl)]
        public string AvatarDataUrl { get; set; }
    }
}
