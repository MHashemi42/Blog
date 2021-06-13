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
                      .HasForeignKey<ApplicationUser>(u => u.AvatarId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ApplicationUser>(user =>
            {
                user.Property(u => u.FriendlyName).HasMaxLength(50).IsRequired();
                user.Property(u => u.BirthDay).HasColumnType("date");
                user.Property(u => u.Location).HasMaxLength(100);
                user.Property(u => u.Bio).HasMaxLength(500);

                user.HasOne(u => u.Avatar)
                    .WithOne(a => a.User)
                    .HasForeignKey<Avatar>(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
