using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blog.Web.ViewModels
{
    public class ReCaptchaResponse
    {
        public bool Success { get; set; }
        public double Score { get; set; }
    }
}
