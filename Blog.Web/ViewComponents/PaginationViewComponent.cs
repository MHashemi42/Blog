using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.ViewComponents
{
    public class PaginationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(bool hasPrevious, bool hasNext, int currentPage)
        {
            string previousDisabled = hasPrevious ? string.Empty : "disabled";
            string nextDisabled = hasNext ? string.Empty : "disabled";

            return View((previousDisabled, nextDisabled, currentPage));
        }
    }
}
