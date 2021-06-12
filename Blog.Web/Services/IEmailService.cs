using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Services
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html);
    }
}
