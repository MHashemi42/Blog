using Blog.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Services
{
    public interface ICaptchaService
    {
        public Task<bool> VerifyReCaptcha(string token);
    }
}
