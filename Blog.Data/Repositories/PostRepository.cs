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
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PagedList<PostSummary>> GetPagedListAsync(PostParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(paramName: nameof(parameters));
            }

            var source = _dbSet.Select(p => new PostSummary
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Description = p.Description,
                CreatedDate = p.CreatedDate,
                AuthorFriendlyName = p.Author.FriendlyName,
                AuthorUserName = p.Author.UserName,
                Labels = p.Labels.Select(x => x.Name)
            })
             .OrderByDescending(p => p.CreatedDate);

            return await PagedList<PostSummary>
                .ToPagedListAsync(source: source, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<bool> IsSlugExist(string slug)
        {
            return await _dbSet.AnyAsync(p => p.Slug == slug);
        }
    }
}
