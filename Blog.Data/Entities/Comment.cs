using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string ChildrenIds { get; set; }
        public int? UserId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }

        public Comment Parent { get; set; }
        public ApplicationUser User { get; set; }
        public Post Post { get; set; }
        public ICollection<Comment> Children { get; set; }
    }
}
