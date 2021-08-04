using AutoMapper;
using Blog.Data;
using Blog.Data.Entities;
using Blog.Data.Models;
using Blog.Data.Repositories;
using Blog.Web.ViewModels;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DNTPersianUtils.Core;

namespace Blog.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public PostController(IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index([FromQuery] PostParameters parameters)
        {
            var posts = await _unitOfWork.PostRepository
                .GetPagedListAsync(parameters); 

            return View(posts);
        }

        [Authorize(Roles = "Admin, Writer")]
        public async Task<IActionResult> Create()
        {
            CreatePostViewModel viewmodel = new()
            {
                Labels = await CreateSelectListLabels()
            };

            return View(viewmodel);
        }

        [Authorize(Roles = "Admin, Writer")]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel viewModel)
        {
            if (ModelState.IsValid is false)
            {
                viewModel.Labels = await CreateSelectListLabels();

                return View(viewModel);
            }

            var isSlugExist = await _unitOfWork.PostRepository
                .IsSlugExist(viewModel.Slug);
            if (isSlugExist)
            {
                ModelState.AddModelError(nameof(CreatePostViewModel.Slug),
                    "اسلاگ مورد نظر از قبل استفاده شده است.");
                viewModel.Labels = await CreateSelectListLabels();

                return View(viewModel);
            }

            List<Label> labels = new();
            foreach (var labelId in viewModel.LabelIds)
            {
                var label = await _unitOfWork.LabelRepository.GetByIdAsync(labelId);
                if (label is null)
                {
                    return BadRequest();
                }
                labels.Add(label);
            }

            var nameIdentifier = User.Claims
                .Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var authorId = int.Parse(nameIdentifier);

            var post = new Post
            {
                AuthorId = authorId,
                Title = viewModel.Title,
                Slug = viewModel.Slug,
                Description = viewModel.Description,
                Body = viewModel.Body,
                IsHidden = viewModel.IsHidden,
                Labels = labels
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

        [Route("[controller]/{id:int}/{slug?}")]
        public async Task<IActionResult> Details(int id, string slug)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (post is null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(slug))
            {
                return RedirectToAction(nameof(Details), new { post.Id, post.Slug });
            }

            if (User.Identity.IsAuthenticated)
            {
                var userIdText = User.Claims
                    .First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var userId = int.Parse(userIdText);

                var isUserSeePost = post.Views.Select(v => v.UserId).Contains(userId);

                if (isUserSeePost is false)
                {
                    post.Views.Add(new View { UserId = userId, PostId = post.Id });
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            var htmlSanitizer = new HtmlSanitizer();
            htmlSanitizer.AllowedAttributes.Add("class");
            htmlSanitizer.AllowedCssProperties.Add("position");
            htmlSanitizer.AllowedAttributes.Add("data-oembed-url");
            htmlSanitizer.AllowedTags.Add("iframe");
            htmlSanitizer.AllowedTags.Add("style");
            var sanitizedBody = htmlSanitizer.Sanitize(post.Body);

            List<Comment> rootComments = post.Comments
                .Where(c => c.ParentId == null)
                .ToList();
            var comments = _mapper.Map<List<ReadCommentViewModel>>(rootComments);

            ReadPostViewModel viewModel = new()
            {
                PostId = post.Id,
                Title = post.Title,
                Description = post.Description,
                Body = new HtmlString(sanitizedBody),
                CreatedDate = post.CreatedDate.ToFriendlyPersianDateTextify(),
                AuthorFriendlyName = post.Author.FriendlyName,
                AuthorUserName = post.Author.UserName,
                AuthorAvatar = post.Author.AvatarName,
                Views = post.Views.Count,
                Labels = post.Labels,
                Comments = comments
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Writer")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdForUpdateAsync(id);
            if (post is null)
            {
                return NotFound();
            }

            UpdatePostViewModel viewModel = new()
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Description = post.Description,
                Body = post.Body,
                Labels = await CreateSelectListLabels(post.Labels.Select(x => x.Id))
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Writer")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePostViewModel viewModel)
        {
            if (ModelState.IsValid is false)
            {
                viewModel.Labels = await CreateSelectListLabels(
                    viewModel.LabelIds ?? Array.Empty<int>());
                return View(viewModel);
            }
            if (viewModel.Id != id)
            {
                return BadRequest();
            }

            var post = await _unitOfWork.PostRepository.GetByIdForUpdateAsync(id);
            if (post is null)
            {
                return NotFound();
            }

            var isSlugExist = await _unitOfWork.PostRepository
                .IsSlugExist(viewModel.Slug);
            var isNewSlug = post.Slug != viewModel.Slug;

            if (isSlugExist && isNewSlug)
            {
                ModelState.AddModelError(nameof(UpdatePostViewModel.Slug),
                    "اسلاگ مورد نظر از قبل استفاده شده است.");

                viewModel.Labels = await CreateSelectListLabels(
                    viewModel.LabelIds ?? Array.Empty<int>());

                return View(viewModel);
            }

            post.Labels.Clear();
            if (viewModel.LabelIds is object)
            {
                foreach (var labelId in viewModel.LabelIds)
                {
                    var label = await _unitOfWork.LabelRepository.GetByIdAsync(labelId);
                    if (label is null)
                    {
                        return BadRequest();
                    }
                    post.Labels.Add(label);
                }
            }

            var nameIdentifier = User.Claims
                .Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var modifierId = int.Parse(nameIdentifier);

            post.Title = viewModel.Title;
            post.Slug = viewModel.Slug;
            post.Description = viewModel.Description;
            post.Body = viewModel.Body;
            post.IsHidden = viewModel.IsHidden;
            post.ModifiedDate = DateTime.UtcNow;
            post.ModifierId = modifierId;

            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { post.Id, post.Slug});
        }

        private async Task<List<SelectListItem>> CreateSelectListLabels(
            IEnumerable<int> selectedLabelIds)
        {
            var labels = await _unitOfWork.LabelRepository.GetAllAsync();

            var selectList = new List<SelectListItem>();
            foreach (var label in labels)
            {
                bool isSelected = selectedLabelIds.Contains(label.Id);
                selectList.Add(new SelectListItem
                {
                    Text = label.Name,
                    Value = label.Id.ToString(),
                    Selected = isSelected
                });
            }

            return selectList;
        }

        private async Task<List<SelectListItem>> CreateSelectListLabels()
        {
            return await CreateSelectListLabels(Enumerable.Empty<int>());
        }
    }
}
