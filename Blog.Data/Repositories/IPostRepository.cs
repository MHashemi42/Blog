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
        Task<Post> GetByIdForUpdateAsync(int postId);
        Task<PagedList<PostSummary>> GetPagedListAsync(PostParameters parameters);
        Task<PagedList<PostSummary>> GetPagedListAsync(PostParameters parameters, string labelSlug);
        Task<bool> IsExist(int postId);
        Task<bool> IsSlugExist(string slug);
        Task<IEnumerable<PostSummary>> GetMostViewedPostsAsync(int postCount);
        Task<IEnumerable<PostSummary>> GetMostDiscussionPostsAsync(int postCount);
    }
}
