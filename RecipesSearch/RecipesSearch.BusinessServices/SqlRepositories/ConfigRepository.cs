using System.Collections.Generic;
using System.Linq;
using RecipesSearch.Data.Models;
using RecipesSearch.BusinessServices.SqlRepositories.Base;

namespace RecipesSearch.BusinessServices.SqlRepositories
{
    public class ConfigRepository : SqlRepositoryBase
    {
        public Config SaveConfig(Config config)
        {
            // Don't update crawled sites here
            config.SitesToCrawl = _dbContext.SitesToCrawl.Where(siteToCrawl => siteToCrawl.ConfigId == config.Id).ToList();
            return SaveEntity(config, _dbContext.Configs);
        }       

        public Config GetConfig()
        {
            return GetConfigs().First();
        }

        private List<Config> GetConfigs()
        {
            var configs = GetEntities(_dbContext.Configs);
            foreach (var config in configs)
            {
                config.SitesToCrawl = config.SitesToCrawl.Where(siteToCrawl => siteToCrawl.IsActive).ToList();
            }
            return configs;
        }
    }
}