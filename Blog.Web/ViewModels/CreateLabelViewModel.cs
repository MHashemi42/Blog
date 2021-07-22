using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class CreateLabelViewModel
    {
        [StringLength(25, MinimumLength = 2, ErrorMessage = "{0} باید بین {1} تا {2} حرف باشد.")]
        [Required(ErrorMessage = "{0} مورد نیاز است.")]
        [Display(Name = "نام برچسب")]
        public string Name { get; set; }
    }
}
