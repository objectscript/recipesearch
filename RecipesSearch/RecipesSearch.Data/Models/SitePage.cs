using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesSearch.Data.Models
{
    public class SitePage
    {
        public int SiteID { get; set; }
        public string URL { get; set; }
        public string Keyword { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
