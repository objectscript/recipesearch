using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.BusinessServices.SqlRepositories.Base;
using RecipesSearch.DAL.Cache.Adapters;

namespace RecipesSearch.BusinessServices.SqlRepositories
{
    public class PageStatsRepository : SqlRepositoryBase
    {
        public int GetTfIdfStatistic()
        {
            try
            {
                using (var cacheAdapter = new SimilarResultsAdapter())
                {
                    return cacheAdapter.GetTfIdfStatistic();
                }
            }
            catch (Exception exception)
            {
                Logger.LogError("PageStatsRepository.GetTfIdfStatistic() failed", exception);
                return -1;
            }
        }

        public int GetNearestResultsStatistic()
        {
            try
            {
                using (var cacheAdapter = new SimilarResultsAdapter())
                {
                    return cacheAdapter.GetNearestResultsStatistic();
                }
            }
            catch (Exception exception)
            {
                Logger.LogError("PageStatsRepository.GetNearestResultsStatistic() failed", exception);
                return -1;
            }
        }
    }
}
