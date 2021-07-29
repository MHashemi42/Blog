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
    public interface IPostRepository : IRepository<Post>
    {
        Task<PagedList<PostSummary>> GetPagedListAsync(PostParameters parameters);
        Task<PagedList<PostSummary>> GetPagedListAsync(PostParameters parameters, string label);
        Task<bool> IsSlugExist(string slug);
    }
}
