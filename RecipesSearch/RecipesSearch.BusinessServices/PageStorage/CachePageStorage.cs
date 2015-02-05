using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.DAL.Cache;
using RecipesSearch.Data.Models;
using RecipesSearch.Data.Views;

namespace RecipesSearch.BusinessServices.PageStorage
{
    public class CachePageStorage : IPageStorage
    {
        private readonly CacheAdapter _cacheAdapter = new CacheAdapter();
        public bool SaveSitePage(SitePage sitePage)
        {
            try
            {
                return _cacheAdapter.AddSitePage(sitePage);
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("CachePageStorage.SaveSitePage failed"), exception);
                return false;
            }     
        }

        public List<SiteInfo> GetSitesInfo()
        {
            try
            {
                return _cacheAdapter.GetSitesInfo();
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("CachePageStorage.GetSitesInfo failed"), exception);
                return null;
            }  
        }

        public bool DeletePages(int siteId)
        {
            try
            {
                return _cacheAdapter.DeleteSiteRecords(siteId);
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("CachePageStorage.DeletePages({0}) failed", siteId), exception);
                return false;
            } 
        }

        public bool DeletePages()
        {
            try
            {
                return _cacheAdapter.DeleteSitesRecords();
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("CachePageStorage.DeletePages failed"), exception);
                return false;
            }
        }
    }
}
