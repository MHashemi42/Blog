using Blog.Data.Entities;
using Blog.Data.Helpers;
using Blog.Data.Models;
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
        Task<PagedList<CommentSummary>> GetUserCommentsPagedListAsync(CommentParameters parameters, int userId);
    }
}
