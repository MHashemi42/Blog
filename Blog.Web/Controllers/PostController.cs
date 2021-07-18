using Blog.Data;
using Blog.Data.Entities;
using Blog.Data.Models;
using Blog.Data.Repositories;
using Blog.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index([FromQuery] PostParameters parameters)
        {
            var posts = await _unitOfWork.PostRepository
                .GetPagedListAsync(parameters); 

            return View(posts);
        }

        [Authorize(Roles = "Admin, Writer")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Writer")]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel viewModel)
        {
            if (ModelState.IsValid is false)
            {
                return View();
            }

            var nameIdentifier = User.Claims
                .Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var authorId = int.Parse(nameIdentifier);

            var post = new Post
            {
                AuthorId = authorId,
                Title = viewModel.Title,
                Description = viewModel.Description,
                Body = viewModel.Body,
                IsHidden = viewModel.IsHidden
            };

            await _unitOfWork.PostRepository.AddAsync(post);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
