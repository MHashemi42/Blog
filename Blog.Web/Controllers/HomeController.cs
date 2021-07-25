using Blog.Data;
using Blog.Data.Helpers;
using Blog.Data.Models;
using Blog.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async  Task<IActionResult> Index([FromQuery] PostParameters parameters)
        {
            PagedList<PostSummary> posts = await _unitOfWork.PostRepository
                .GetPagedListAsync(parameters);

            return View(posts);
        }
    }
}
