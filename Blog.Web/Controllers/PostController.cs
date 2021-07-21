﻿using Blog.Data;
using Blog.Data.Entities;
using Blog.Data.Models;
using Blog.Data.Repositories;
using Blog.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostController(IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
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

        [Authorize(Roles = "Admin, Writer")]
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload is null)
            {
                return BadRequest();
            }

            var directoryPath = Path
                .Combine(_webHostEnvironment.WebRootPath,"images", "posts");
            if (Directory.Exists(directoryPath) is false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            var ext = Path.GetExtension(upload.FileName);
            var fileName = Guid.NewGuid().ToString() + ext;
            var filePath = Path.Combine(directoryPath, fileName);

            using var stream = System.IO.File.Create(filePath);
            await upload.CopyToAsync(stream);

            var url = Path.Combine("/images", "posts", fileName);

            var success = new
            {
                Uploaded = 1,
                Url = url,
                FileName = fileName
            };

            return Ok(success);
        }
    }
}
