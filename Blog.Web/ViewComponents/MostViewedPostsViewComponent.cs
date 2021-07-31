using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewComponents
{
    public class MostViewedPostsViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public MostViewedPostsViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = await _unitOfWork.PostRepository
                .GetMostViewedPostsAsync(postCount: 5);

            return View(posts);
        }
    }
}
