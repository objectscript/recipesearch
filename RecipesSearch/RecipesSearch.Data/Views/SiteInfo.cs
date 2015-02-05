using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesSearch.Data.Views
{
    public class SiteInfo
    {
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public int PageCount { get; set; }
    }
}
