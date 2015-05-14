using System.Collections.Generic;
using System.Linq;
using RecipesSearch.DAL.Cache.Adapters.Base;
using RecipesSearch.Data.Models;
using RecipesSearch.BusinessServices.SqlRepositories.Base;

namespace RecipesSearch.BusinessServices.SqlRepositories
{
    public class ConfigRepository : SqlRepositoryBase
    {
        public Config SaveConfig(Config config)
        {
            return SaveEntity(config);
        }       

        public Config GetConfig()
        {
            return GetConfigs().First();
        }

        private List<Config> GetConfigs()
        {
            var configs = GetEntities<Config>();
            using (var cacheAdapter = new CacheAdapter())
            {
                foreach (var config in configs)
                {
                    config.SitesToCrawl = cacheAdapter
                        .GetEntities<SiteToCrawl>()
                        .Where(siteToCrawl => siteToCrawl.ConfigId == config.Id)
                        .ToList();
                }
            }
            
            return configs;
        }
    }
}