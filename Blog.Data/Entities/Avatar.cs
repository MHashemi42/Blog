using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Entities
{
    public class Avatar
    {
        public int AvatarId { get; set; }
        public string ImageTitle { get; set; }
        public byte[] ImageData { get; set; }
        public int UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
