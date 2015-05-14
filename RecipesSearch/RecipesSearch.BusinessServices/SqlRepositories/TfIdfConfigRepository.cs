using System.Collections.Generic;
using System.Linq;
using RecipesSearch.DAL.Cache.Adapters.Base;
using RecipesSearch.Data.Models;
using RecipesSearch.BusinessServices.SqlRepositories.Base;

namespace RecipesSearch.BusinessServices.SqlRepositories
{
    public class TfIdfConfigRepository : SqlRepositoryBase
    {
        public TfIdfConfig SaveConfig(TfIdfConfig config)
        {
            return SaveEntity(config);
        }

        public TfIdfConfig GetConfig()
        {
            return GetConfigs().First();
        }

        private List<TfIdfConfig> GetConfigs()
        {
            var configs = GetEntities<TfIdfConfig>();

            return configs;
        }
    }
}