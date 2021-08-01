using Blog.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<bool> IsExist(int commentId);
    }
}
