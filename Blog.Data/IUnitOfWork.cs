using Blog.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data
{
    public interface IUnitOfWork
    {
        IPostRepository PostRepository { get; }
        ILabelRepository LabelRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
