using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.Data.Models;
using RecipesSearch.Data.Views;

namespace RecipesSearch.WebApplication.ViewModels
{
    public class CrawlerControlViewModel
    {
        public bool IsCrawlingStarted { get; set; }
        public int CrawledPages { get; set; }
        public List<SiteInfo> SitesInfo { get; set; }
        public List<SiteToCrawl> SitesQueue { get; set; } 
    }
}
