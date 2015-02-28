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
            return SaveEntity(siteToCrawl);
        }

        public List<SiteToCrawl> GetSitesToCrawl()
        {
            return GetEntities<SiteToCrawl>();
        }

        public SiteToCrawl GetSiteToCrawl(int siteId)
        {
            return GetEntityById<SiteToCrawl>(siteId);
        }

        public bool RemoveSiteToCrawl(int id)
        {
            return DeleteEntity<SiteToCrawl>(id);
        }

        public List<SiteToCrawl> GetSitesToCrawlForConfig(int configId)
        {
            return GetEntities<SiteToCrawl>()
                .Where(siteToCrawl => siteToCrawl.ConfigId == configId)
                .ToList();
        }
    }
}