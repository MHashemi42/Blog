﻿using Blog.Data.Entities;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<ApplicationUser>(user =>
            {
                user.Property(u => u.FriendlyName)
                    .HasMaxLength(50)
                    .IsRequired();

                user.Property(u => u.BirthDay)
                    .HasColumnType("date");

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

                label.HasMany(l => l.Posts)
                     .WithMany(p => p.Labels);
            });
        }
    }
}
