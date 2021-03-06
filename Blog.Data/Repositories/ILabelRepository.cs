using Blog.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Repositories
{
    public interface ILabelRepository : IRepository<Label>
    {
        Task<bool> IsNameExist(string name);
        Task<bool> IsSlugExist(string slug);
        Task<IEnumerable<Label>> GetAllByPostId(int postId);
    }
}
