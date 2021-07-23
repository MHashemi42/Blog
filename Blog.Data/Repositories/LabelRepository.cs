using Blog.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Repositories
{
    public class LabelRepository : Repository<Label>, ILabelRepository
    {
        public LabelRepository(BlogDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Label>> GetAllByPostId(int postId)
        {
            var labels = await _dbcontext.Posts
                .Where(p => p.Id == postId)
                .SelectMany(p => p.Labels)
                .ToListAsync();

            return labels;
        }

        public async Task<bool> IsExist(string name)
        {
            return await _dbSet.AnyAsync(label => label.Name == name);
        }
    }
}
