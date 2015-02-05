using System;
using System.Collections.Generic;
using System.Linq;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.Data.Models;
using RecipesSearch.BusinessServices.SqlRepositories.Base;

namespace RecipesSearch.BusinessServices.SqlRepositories
{
    public class CrawlingHistoryRepository : SqlRepositoryBase
    {
        public CrawlingHistoryItem SaveCrawlingHistoryItem(CrawlingHistoryItem crawlingHistoryItem)
        {
            return SaveEntity(crawlingHistoryItem, _dbContext.CrawlingHistory);
        }

        // Returns top 20 items
        public List<CrawlingHistoryItem> GetCrawlingHistory()
        {
            return GetEntities(_dbContext.CrawlingHistory).Take(20).ToList();
        }

        public bool Clear()
        {
            try
            {
                foreach (var crawlingHistoryItem in _dbContext.CrawlingHistory)
                {
                    crawlingHistoryItem.IsActive = false;
                }
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                Logger.LogError("CrawlingHistoryRepository.Clear() failed", exception);
                return false;;
            }           
        }
    }
}