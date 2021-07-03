using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FriendlyName { get; set; }
        public DateTime? BirthDay { get; set; }
        public string Location { get; set; }
        public string Bio { get; set; }
        public string AvatarPath { get; set; }
    }
}
