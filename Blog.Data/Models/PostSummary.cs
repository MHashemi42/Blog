using Blog.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Models
{
    public class PostSummary
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string AuthorFriendlyName { get; set; }
        public string AuthorUserName { get; set; }
        public IEnumerable<Label> Labels { get; set; }
    }
}
