using AutoMapper;
using Blog.Data.Entities;
using Blog.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, ReadCommentViewModel>();
        }
    }
}
