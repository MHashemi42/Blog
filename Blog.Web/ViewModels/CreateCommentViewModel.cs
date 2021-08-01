using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class CreateCommentViewModel
    {
        public int? ParentId { get; init; }

        public int PostId { get; set; }

        [Required(ErrorMessage = "{0} مورد نیاز است.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "{0} باید بین {2} تا {1} حرف باشد")]
        [Display(Name = "متن دیدگاه")]
        public string Content { get; init; }
    }
}
