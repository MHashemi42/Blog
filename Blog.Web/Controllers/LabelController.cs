using Blog.Data;
using Blog.Data.Entities;
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
        public async Task<IActionResult> GetLabels()
        {
            var labels = await _unitOfWork.LabelRepository.GetAllAsync();

            return Ok(labels);
        }

        [Authorize(Roles = "Admin, Writer")]
        public async Task<IActionResult> GetLabelsByPostId(int postId)
        {
            var labels = await _unitOfWork.LabelRepository.GetAllByPostId(postId);

            return Ok(labels);
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

            var isExist = await _unitOfWork.LabelRepository.IsExist(viewModel.Name);
            if (isExist)
            {
                ModelState.AddModelError(nameof(viewModel.Name), "نام برچسب تکراری است.");
                return View();
            }

            Label label = new()
            {
                Name = viewModel.Name
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
                Name = label.Name
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

            var isExist = await _unitOfWork.LabelRepository.IsExist(viewModel.Name);
            if (isExist)
            {
                ModelState.AddModelError(nameof(viewModel.Name), "نام برچسب تکراری است.");
                return View();
            }

            label.Name = viewModel.Name;
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
