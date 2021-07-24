using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class UpdatePostViewModel
    {
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 10, ErrorMessage = "{0} باید بین {1} تا {2} حرف باشد.")]
        [Required(ErrorMessage = "{0} مورد نیاز است.")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$",
            ErrorMessage = "{0} تنها میتواند شامل حروف کوچک، اعداد و - باشد.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "{0} باید بین {1} تا {2} حرف باشد.")]
        [Required(ErrorMessage = "{0} مورد نیاز است.")]
        [Display(Name = "اسلاگ")]
        public string Slug { get; set; }


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

        public List<SelectListItem> Labels { get; set; }
        public int[] LabelIds { get; set; } = Array.Empty<int>();
    }
}
