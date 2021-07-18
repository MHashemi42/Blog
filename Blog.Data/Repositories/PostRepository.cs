using Blog.Data.Entities;
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

        public override Task AddAsync(Post entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            return base.AddAsync(entity);
        }
    }
}
