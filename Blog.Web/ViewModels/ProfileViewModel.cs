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
        [Display(Name = "تصویر پروفایل")]
        public IFormFile Avatar { get; set; }
    }
}
