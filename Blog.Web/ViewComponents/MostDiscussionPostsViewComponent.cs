using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewComponents
{
    public class MostDiscussionPostsViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public MostDiscussionPostsViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = await _unitOfWork.PostRepository
                .GetMostDiscussionPostsAsync(postCount: 5);

            return View(posts);
        }
    }
}
