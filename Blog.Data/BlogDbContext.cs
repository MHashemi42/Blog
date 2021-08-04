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

        public DbSet<Post> Posts { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<View> Views { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<ApplicationUser>(user =>
            {
                user.Property(u => u.FriendlyName)
                    .HasMaxLength(50)
                    .IsRequired();

                user.Property(u => u.Location)
                    .HasMaxLength(100);

                user.Property(u => u.Bio)
                    .HasMaxLength(500);

                user.Property(u => u.AvatarName)
                    .HasMaxLength(500);
            });

            builder.Entity<Post>(post =>
            {
                post.HasKey(p => p.Id);

                post.Property(p => p.Title)
                    .HasMaxLength(100)
                    .IsRequired();

                post.Property(p => p.Slug)
                    .HasMaxLength(100)
                    .IsRequired();

                post.HasIndex(p => p.Slug)
                    .IsUnique();

                post.Property(p => p.Description)
                    .HasMaxLength(200)
                    .IsRequired();

                post.Property(p => p.Body)
                    .IsRequired();

                post.HasOne(p => p.Author)
                    .WithMany(a => a.Posts)
                    .HasForeignKey(p => p.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                post.HasOne(p => p.Modifier)
                    .WithMany(m => m.ModifiedPost)
                    .HasForeignKey(p => p.ModifierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Label>(label =>
            {
                label.HasKey(l => l.Id);

                label.Property(l => l.Name)
                     .HasMaxLength(25)
                     .IsRequired();

                label.HasIndex(l => l.Name)
                     .IsUnique();

                label.Property(l => l.Slug)
                     .HasMaxLength(30)
                     .IsRequired();

                label.HasIndex(l => l.Slug)
                     .IsUnique();

                label.HasMany(l => l.Posts)
                     .WithMany(p => p.Labels);
            });

            builder.Entity<Comment>(comment =>
            {
                comment.HasKey(c => c.Id);

                comment.Property(c => c.Content)
                       .HasMaxLength(1000)
                       .IsRequired();

                comment.HasOne(c => c.Parent)
                       .WithMany(c => c.Children)
                       .HasForeignKey(c => c.ParentId)
                       .OnDelete(DeleteBehavior.Restrict);

                comment.HasOne(c => c.Post)
                       .WithMany(p => p.Comments)
                       .HasForeignKey(c => c.PostId)
                       .OnDelete(DeleteBehavior.Cascade);

                comment.HasOne(c => c.User)
                       .WithMany(u => u.Comments)
                       .HasForeignKey(c => c.UserId)
                       .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<View>(view =>
            {
                view.HasKey(v => v.Id);

                view.HasOne(v => v.User)
                    .WithMany(u => u.Views)
                    .HasForeignKey(v => v.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                view.HasOne(v => v.Post)
                    .WithMany(p => p.Views)
                    .HasForeignKey(v => v.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
