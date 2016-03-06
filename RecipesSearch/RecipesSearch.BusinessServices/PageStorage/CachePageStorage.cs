using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.DAL.Cache;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.Data.Models;
using RecipesSearch.Data.Views;

namespace RecipesSearch.BusinessServices.PageStorage
{
    public class CachePageStorage : IPageStorage
    {
        private readonly SitePageAdapter _cacheAdapter = new SitePageAdapter();
        public bool SaveSitePage(
            SitePage sitePage, 
            bool processKeywords,
            int extendedKeywordsMinWordCount,
            bool extendedKeywordsUseFilter,
            bool updateSpellcheckDict,
            bool buildTf,
            string tfBuilderName)
        {
            try
            {
                return _cacheAdapter.AddSitePage(sitePage, processKeywords, extendedKeywordsMinWordCount, extendedKeywordsUseFilter, updateSpellcheckDict, buildTf, tfBuilderName);
            }
            catch (Exception exception)
            {
                LoggerWrapper.LogError(String.Format("CachePageStorage.SaveSitePage failed"), exception);
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
                LoggerWrapper.LogError(String.Format("CachePageStorage.GetSitesInfo failed"), exception);
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
                LoggerWrapper.LogError(String.Format("CachePageStorage.DeletePages({0}) failed", siteId), exception);
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
                LoggerWrapper.LogError(String.Format("CachePageStorage.DeletePages failed"), exception);
                return false;
            }
        }

        public void Dispose()
        {
            _cacheAdapter.Dispose();
        }
    }
}
