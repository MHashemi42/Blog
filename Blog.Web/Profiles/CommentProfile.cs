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
            CreateMap<Comment, ReadCommentViewModel>()
                .ForMember(r => r.ParentFriendlyName, c => c.MapFrom(c => c.Parent.User.FriendlyName))
                .ForMember(r => r.ParentUserName, c => c.MapFrom(c => c.Parent.User.UserName))
                .ForMember(r => r.FriendlyName, c => c.MapFrom(c => c.User.FriendlyName))
                .ForMember(r => r.UserName, c => c.MapFrom(c => c.User.UserName))
                .ForMember(r => r.AvatarName, c => c.MapFrom(c => c.User.AvatarName));
        }
    }
}
