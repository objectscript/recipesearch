using System.Collections.Generic;
using System.Linq;
using RecipesSearch.Data.Models;
using RecipesSearch.BusinessServices.SqlRepositories.Base;

namespace RecipesSearch.BusinessServices.SqlRepositories
{
    public class SiteToCrawlRepository : SqlRepositoryBase
    {
        public SiteToCrawl SaveSiteToCrawl(SiteToCrawl siteToCrawl)
        {
            return SaveEntity(siteToCrawl, _dbContext.SitesToCrawl);
        }

        public List<SiteToCrawl> GetSitesToCrawl()
        {
            return GetEntities(_dbContext.SitesToCrawl);
        }

        public SiteToCrawl GetSiteToCrawl(int siteId)
        {
            return GetEntityById(siteId, _dbContext.SitesToCrawl);
        }

        public bool RemoveSiteToCrawl(int id)
        {
            return DeleteEntity(id, _dbContext.SitesToCrawl);
        }

        public List<SiteToCrawl> GetSitesToCrawlForConfig(int configId)
        {
            return GetEntities(_dbContext.SitesToCrawl)
                .Where(siteToCrawl => siteToCrawl.ConfigId == configId)
                .ToList();
        }
    }
}