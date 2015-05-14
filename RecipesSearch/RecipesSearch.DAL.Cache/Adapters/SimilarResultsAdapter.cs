using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using InterSystems.Data.CacheClient;
using RecipesSearch.DAL.Cache.Adapters.Base;
using RecipesSearch.DAL.Cache.Utilities;
using RecipesSearch.Data.Models;
using RecipesSearch.Data.Views;

namespace RecipesSearch.DAL.Cache.Adapters
{
    public class SimilarResultsAdapter : CacheAdapter
    {
        public List<SitePageTfIdf> GetWordsTfIdf()
        {
            var command = new CacheCommand(GetFullProcedureName("SitePage_GetTFIDF"), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            var dataReader = command.ExecuteReader();

            var tfIdf = ObjectMapper.Map<SitePageTfIdf>(dataReader);

            return tfIdf;
        }

        public bool UpdateSimilarResults(int pageId, IEnumerable<int> results)
        {
            EnsureConnectionOpened();

            var command = new CacheCommand(GetFullProcedureName("SitePage_UpdateNearestResults"), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("PageId", pageId);
            command.Parameters.Add("NearestResults", String.Join(" ", results)); 

            return command.ExecuteNonQuery() != 0;
        }    

        public int GetNearestResultsStatistic()
        {
            var command = new CacheCommand(GetFullProcedureName("SitePage_GetNearestResultsStatistic"), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            var result = command.ExecuteScalar();

            return (int)(decimal)result;
        }      
    }
}
