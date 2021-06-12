using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class ConfirmEmailViewModel
    {
        [Required]
        public string Code { get; set; }
        public int UserId { get; set; }
    }
}
