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

        public DbSet<Avatar> Avatars { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Avatar>(avatar =>
            {
                avatar.HasKey(a => a.AvatarId);

                avatar.Property(a => a.ImageTitle).IsRequired();
                avatar.Property(a => a.ImageData).IsRequired();

                avatar.HasOne(a => a.User)
                      .WithOne(u => u.Avatar)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ApplicationUser>(user =>
            {
                user.HasOne(u => u.Avatar)
                    .WithOne(a => a.User)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
