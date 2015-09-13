using System.Collections.Generic;
using System.Linq;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.Data.Models;
using RecipesSearch.BusinessServices.SqlRepositories.Base;

namespace RecipesSearch.BusinessServices.SqlRepositories
{
    public class SearchSettingsRepository : SqlRepositoryBase
    {
        public SearchSettings SaveSearchSettings(SearchSettings searchSettings)
        {
            return SaveEntity(searchSettings);
        }

        public SearchSettings GetSearchSettings()
        {
            return GetEntities<SearchSettings>().First();
        }

        public List<string> GetOnlineTfIdfBuilders()
        {
            using (var adapter = new TfIdfAdapter())
            {
                return adapter.GetOnlineTfIdfBuilders();
            }
        } 
    }
}