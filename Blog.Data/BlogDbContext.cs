using Blog.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data
{
    public class BlogDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options)
            : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<ApplicationUser>(user =>
            {
                user.Property(u => u.FriendlyName).HasMaxLength(50).IsRequired();
                user.Property(u => u.BirthDay).HasColumnType("date");
                user.Property(u => u.Location).HasMaxLength(100);
                user.Property(u => u.Bio).HasMaxLength(500);
                user.Property(u => u.AvatarName).HasMaxLength(500);
            });
        }
    }
}
