using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class ReadCommentViewModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string UserUserName { get; set; }
        public string UserFriendlyName { get; set; }
        public string UserAvatarName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
        public IEnumerable<ReadCommentViewModel> Children { get; set; }
    }
}
