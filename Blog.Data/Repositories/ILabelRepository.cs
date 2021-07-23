﻿using Blog.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Repositories
{
    public interface ILabelRepository : IRepository<Label>
    {
        Task<bool> IsExist(string name);
        Task<IEnumerable<Label>> GetAllByPostId(int postId);
    }
}
