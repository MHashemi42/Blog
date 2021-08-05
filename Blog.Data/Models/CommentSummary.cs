using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Models
{
    public class CommentSummary
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string Content { get; set; }
        public string UserFriendlyName { get; set; }
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostSlug { get; set; }
    }
}
