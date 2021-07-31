using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public bool IsHidden { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int AuthorId { get; set; }
        public int? ModifierId { get; set; }

        public ApplicationUser Author { get; set; }
        public ApplicationUser Modifier { get; set; }
        public ICollection<Label> Labels { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<View> Views { get; set; }
    }
}
