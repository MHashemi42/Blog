using Blog.Data;
using Blog.Data.Entities;
using Blog.Web.Extensions;
using Blog.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentViewModel viewModel)
        {
            if (ModelState.IsValid is false)
            {
                return BadRequest();
            }

            var isPostExist = await _unitOfWork.PostRepository.IsExist(viewModel.PostId);
            if (isPostExist is false)
            {
                return BadRequest();
            }

            if (viewModel.ParentId.HasValue)
            {
                var isParentExist = await _unitOfWork.CommentRepository.IsExist(viewModel.ParentId.Value);
                if (isParentExist is false)
                {
                    return BadRequest();
                }
            }

            Claim NameIdentifier = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
            var userId = int.Parse(NameIdentifier.Value);
            var newComment = new Comment
            {
                ParentId = viewModel.ParentId,
                PostId = viewModel.PostId,
                UserId = userId,
                Content = viewModel.Content
            };

            await _unitOfWork.CommentRepository.AddAsync(newComment);
            await _unitOfWork.SaveChangesAsync();

            if (viewModel.ParentId.HasValue)
            {
                Comment parentComment = await _unitOfWork.CommentRepository
                    .GetByIdAsync(viewModel.ParentId.Value);

                List<int> childrenIdsOfParent = new();
                
                if (string.IsNullOrWhiteSpace(parentComment.ChildrenIds))
                {
                    childrenIdsOfParent.Add(newComment.Id);
                }
                else
                {
                    List<int> currentChildrenIdsOfParent = parentComment.ChildrenIds
                        .CommaSeparatedStringToIntList();

                    childrenIdsOfParent.AddRange(currentChildrenIdsOfParent);
                    childrenIdsOfParent.Add(newComment.Id);
                }

                parentComment.ChildrenIds = string.Join(',', childrenIdsOfParent);
                await _unitOfWork.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
