using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class ReadPostViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public HtmlString Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorFriendlyName { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorAvatar { get; set; }
        public IEnumerable<string> Labels { get; set; }
    }
}
