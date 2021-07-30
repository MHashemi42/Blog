using Blog.Data;
using Blog.Data.Entities;
using Blog.Data.Helpers;
using Blog.Data.Models;
using Blog.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Controllers
{
    public class LabelController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LabelController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Admin, Writer")]
        public async Task<IActionResult> Index()
        {
            var labels = await _unitOfWork.LabelRepository.GetAllAsync();
            return View(labels);
        }

        [Route("label/details/{labelSlug}")]
        public async Task<IActionResult> Details([FromQuery] PostParameters parameters, string labelSlug)
        {
            PagedList<PostSummary> postsByLabel = await _unitOfWork.PostRepository
                .GetPagedListAsync(parameters, labelSlug);

            if (postsByLabel.Count < 1)
            {
                return NotFound();
            }

            ViewBag.Label = postsByLabel.First()
                .Labels.Single(x => x.Slug.Equals(labelSlug,
                    StringComparison.OrdinalIgnoreCase)).Name;

            return View(postsByLabel);
        }

        [Authorize(Roles = "Admin, Writer")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Writer")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateLabelViewModel viewModel)
        {
            if (ModelState.IsValid is false)
            {
                return View();
            }

            var isNameExist = await _unitOfWork.LabelRepository.IsNameExist(viewModel.Name);
            if (isNameExist)
            {
                ModelState.AddModelError(nameof(CreateLabelViewModel.Name), "نام برچسب تکراری است.");
                return View();
            }

            var isSlugExist = await _unitOfWork.LabelRepository.IsSlugExist(viewModel.Slug);
            if (isSlugExist)
            {
                ModelState.AddModelError(nameof(CreateLabelViewModel.Slug), "اسلاگ تکراری است.");
                return View();
            }

            Label label = new()
            {
                Name = viewModel.Name,
                Slug = viewModel.Slug
            };

            await _unitOfWork.LabelRepository.AddAsync(label);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin, Writer")]
        public async Task<IActionResult> Edit(int id)
        {
            var label = await _unitOfWork.LabelRepository.GetByIdAsync(id);
            if (label is null)
            {
                return NotFound();
            }

            UpdateLabelViewModel viewModel = new()
            {
                Id = label.Id,
                Name = label.Name,
                Slug = label.Slug
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Writer")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateLabelViewModel viewModel)
        {
            if (ModelState.IsValid is false)
            {
                return View();
            }

            if (viewModel.Id != id)
            {
                return BadRequest();
            }

            var label = await _unitOfWork.LabelRepository.GetByIdAsync(id);
            if (label is null)
            {
                return NotFound();
            }

            var isNameExist = await _unitOfWork.LabelRepository.IsNameExist(viewModel.Name);
            var isNewName = label.Name != viewModel.Name;
            if (isNameExist && isNewName)
            {
                ModelState.AddModelError(nameof(UpdateLabelViewModel.Name), "نام برچسب تکراری است.");
                return View();
            }

            var isSlugExist = await _unitOfWork.LabelRepository.IsSlugExist(viewModel.Slug);
            var isNewSlug = label.Slug != viewModel.Slug;
            if (isSlugExist && isNewSlug)
            {
                ModelState.AddModelError(nameof(UpdateLabelViewModel.Slug), "نام اسلاگ تکراری است.");
                return View();
            }

            label.Name = viewModel.Name;
            label.Slug = viewModel.Slug;

            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
