using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.DAL.Cache;
using RecipesSearch.Data.Models;
using RecipesSearch.Data.Views;

namespace RecipesSearch.BusinessServices.PageStorage
{
    public class SitePageManager
    {
        private readonly SiteToCrawlRepository _siteToCrawlRepository = new SiteToCrawlRepository();

        public List<SiteInfo> GetSitesInfo()
        {
            try
            {
                using (var pageStorage = new CachePageStorage())
                {
                    var savedSitesInfo = pageStorage.GetSitesInfo();
                    var sites = _siteToCrawlRepository.GetSitesToCrawl();

                    var sitesInfo = sites.GroupJoin(savedSitesInfo,
                        site => site.Id,
                        siteInfo => siteInfo.SiteId,
                        (site, siteInfos) =>
                        {
                            var siteInfo = siteInfos.FirstOrDefault() ?? new SiteInfo { SiteId = site.Id, PageCount = 0 };
                            siteInfo.SiteName = site.Name;
                            siteInfo.SiteURL = site.URL;
                            return siteInfo;
                        });

                    return sitesInfo.ToList();
                }
            }
            catch (Exception exception)
            {
                LoggerWrapper.LogError(String.Format("SitePageManager.GetSitesInfo failed"), exception);
                return null;
            }
        }

        public bool DeletePages(int siteId)
        {
            using (var pageStorage = new CachePageStorage())
            {
                return pageStorage.DeletePages(siteId);
            }          
        }

        public bool DeletePages()
        {
            using (var pageStorage = new CachePageStorage())
            {
                return pageStorage.DeletePages();
            }          
        }
    }
}
