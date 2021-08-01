using Blog.Data.Entities;
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

        public async Task<bool> IsExist(int commentId)
        {
            return await _dbSet.AnyAsync(c => c.Id == commentId);
        }
    }
}
