using Blog.Data.Entities;
using Blog.Data.Helpers;
using Blog.Data.Models;
using Blog.Data.Models.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(BlogDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task AddAsync(Post entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await base.AddAsync(entity);
        }

        public override async Task<Post> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Labels)
                .Include(p => p.Author)
                .Include(p => p.Views)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PostSummary>> GetMostViewedPostsAsync(int postCount)
        {
            return await _dbSet
                .OrderByDescending(p => p.Views.Count)
                .Take(postCount)
                .Select(p => new PostSummary
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug
                })
                .ToListAsync();
        }

        public async Task<PagedList<PostSummary>> GetPagedListAsync(
            PostParameters parameters, string labelSlug = "")
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(paramName: nameof(parameters));
            }

            IQueryable<Post> posts = _dbSet;
            if (string.IsNullOrWhiteSpace(labelSlug) is false)
            {
                posts = posts.Where(p => p.Labels.Any(x => x.Slug == labelSlug));
            }

            IQueryable<PostSummary> source = posts
            .Select(p => new PostSummary
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Description = p.Description,
                CreatedDate = p.CreatedDate,
                AuthorFriendlyName = p.Author.FriendlyName,
                AuthorUserName = p.Author.UserName,
                Labels = p.Labels
            })
              .OrderByDescending(p => p.CreatedDate);

            return await PagedList<PostSummary>
                .ToPagedListAsync(source, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<PagedList<PostSummary>> GetPagedListAsync(PostParameters parameters)
        {
            return await GetPagedListAsync(parameters, string.Empty);
        }

        public async Task<bool> IsExist(int postId)
        {
            return await _dbSet.AnyAsync(p => p.Id == postId);
        }

        public async Task<bool> IsSlugExist(string slug)
        {
            return await _dbSet.AnyAsync(p => p.Slug == slug);
        }
    }
}
