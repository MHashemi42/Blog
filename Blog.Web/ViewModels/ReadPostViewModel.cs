using Blog.Data.Entities;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class ReadPostViewModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public HtmlString Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorFriendlyName { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorAvatar { get; set; }
        public int Views { get; set; }
        public IEnumerable<Label> Labels { get; set; }
        public IEnumerable<ReadCommentViewModel> Comments { get; set; }
    }
}
