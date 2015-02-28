using System;
using System.Collections.Generic;
using System.Linq;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.DAL.Cache.Adapters.Base;
using RecipesSearch.Data.Models;
using RecipesSearch.BusinessServices.SqlRepositories.Base;

namespace RecipesSearch.BusinessServices.SqlRepositories
{
    public class CrawlingHistoryRepository : SqlRepositoryBase
    {
        public CrawlingHistoryItem SaveCrawlingHistoryItem(CrawlingHistoryItem crawlingHistoryItem)
        {
            return SaveEntity(crawlingHistoryItem);
        }

        // Returns top 20 items
        public List<CrawlingHistoryItem> GetCrawlingHistory()
        {
            var crawlingHistoryItems = GetEntities<CrawlingHistoryItem>().Take(20).ToList();
            var sitesToCrawl = GetEntities<SiteToCrawl>();

            crawlingHistoryItems = crawlingHistoryItems.Join(
                sitesToCrawl,
                historyItem => historyItem.SiteId,
                siteToCrawl => siteToCrawl.Id,
                (historyItem, siteToCrawl) =>
                {
                    historyItem.SiteToCrawl = siteToCrawl;
                    return historyItem;
                })
                .ToList();

            return crawlingHistoryItems;
        }

        public bool Clear()
        {
            try
            {
                using (var crawlinHistoryAdapter = new CrawlingHistoryAdapter())
                {
                    crawlinHistoryAdapter.ClearAll();
                }
                return true;
            }
            catch (Exception exception)
            {
                Logger.LogError("CrawlingHistoryRepository.Clear() failed", exception);
                return false;
            }
        }
    }
}