﻿using Blog.Data.Entities;
using Blog.Data.Helpers;
using Blog.Data.Models;
using Blog.Data.Models.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(BlogDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task AddAsync(Post entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await base.AddAsync(entity);
        }

        public async Task<PagedList<Post>> GetPagedListAsync(PostParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(paramName: nameof(parameters));
            }
            
            return await PagedList<Post>
                .ToPagedListAsync(source: _dbSet, parameters.PageNumber, parameters.PageSize);
        }
    }
}