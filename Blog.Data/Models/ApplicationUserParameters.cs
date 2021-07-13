using Blog.Data.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Models
{
    public class ApplicationUserParameters : QueryStringParameters
    {
        [Display(Name = "بر اساس نام کاربری")]
        public bool QueryByUsername { get; set; } = true;
        
        [Display(Name = "بر اساس آدرس ایمیل")]
        public bool QueryByEmail { get; set; }
    }
}
