using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Entities
{
    public class View
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int PostId { get; set; }
        public ApplicationUser User { get; set; }
        public Post Post { get; set; }
    }
}
