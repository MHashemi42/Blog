using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class CreatePostViewModel
    {
        [StringLength(100, MinimumLength = 10, ErrorMessage = "{0} باید بین {1} تا {2} حرف باشد.")]
        [Required(ErrorMessage = "{0} مورد نیاز است.")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [StringLength(200, MinimumLength = 10, ErrorMessage = "{0} باید بین {1} تا {2} حرف باشد")]
        [Required(ErrorMessage = "{0} مورد نیاز است.")]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [MinLength(10, ErrorMessage = "{0} حداقل باید {1} حرف باشد.")]
        [Required(ErrorMessage = "{0} مورد نیاز است.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "متن")]
        public string Body { get; set; }

        [Display(Name = "نمایش پست")]
        public bool IsHidden { get; set; } = true;

        public int[] LabelIds { get; set; } = Array.Empty<int>();
    }
}
