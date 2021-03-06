using Blog.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogDbContext _dbContext;

        public UnitOfWork(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IPostRepository _postRepository;
        public IPostRepository PostRepository
        {
            get
            {
                if (_postRepository is null)
                {
                    _postRepository = new PostRepository(_dbContext);
                }

                return _postRepository;
            }
        }

        private ILabelRepository _labelRepository;
        public ILabelRepository LabelRepository
        {
            get
            {
                if (_labelRepository is null)
                {
                    _labelRepository = new LabelRepository(_dbContext);
                }

                return _labelRepository;
            }
        }

        private ICommentRepository _commentRepository;
        public ICommentRepository CommentRepository
        {
            get
            {
                if (_commentRepository is null)
                {
                    _commentRepository = new CommentRepository(_dbContext);
                }

                return _commentRepository;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
