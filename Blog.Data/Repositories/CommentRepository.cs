using Blog.Data.Entities;
using Blog.Data.Helpers;
using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(BlogDbContext dbContext)
            : base(dbContext)
        {
        }

        public override Task AddAsync(Comment entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            return base.AddAsync(entity);
        }

        public Task<PagedList<CommentSummary>> GetUserCommentsPagedListAsync(CommentParameters parameters, int userId)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(paramName: nameof(parameters));
            }

            var source = _dbSet
                .Where(c => c.UserId == userId)
                .Select(c => new CommentSummary
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedDate = c.CreatedDate,
                    UserFriendlyName = c.User.FriendlyName,
                    PostId = c.Post.Id,
                    PostTitle = c.Post.Title,
                    PostSlug = c.Post.Slug
                })
                .OrderByDescending(c => c.CreatedDate);

            return PagedList<CommentSummary>
                .ToPagedListAsync(source, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<bool> IsExist(int commentId)
        {
            return await _dbSet.AnyAsync(c => c.Id == commentId);
        }
    }
}
