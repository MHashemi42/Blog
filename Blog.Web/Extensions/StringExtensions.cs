using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Web.Extensions
{
    public static class StringExtensions
    {
        public static List<int> CommaSeparatedStringToIntList(this string str)
        {
            string[] numbersInStringArray = str.Split(',');
            List<int> numbersInIntList = numbersInStringArray.Select(int.Parse).ToList();

            return numbersInIntList;
        }
    }
}
