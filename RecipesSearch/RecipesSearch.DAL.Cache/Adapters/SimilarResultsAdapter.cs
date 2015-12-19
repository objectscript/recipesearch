using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using InterSystems.Data.CacheClient;
using RecipesSearch.Data.Framework;
using RecipesSearch.Data.Models;
using RecipesSearch.DAL.Cache.Adapters.Base;
using RecipesSearch.DAL.Cache.Utilities;
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

        public List<NearestResult> GetNearestResults()
        {
            var command = new CacheCommand(GetFullProcedureName("NearestResult_GetSimilarResults", Constants.RecipeDataCachePackage), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            var dataReader = command.ExecuteReader();

            var nearestResults = ObjectMapper.Map<NearestResult>(dataReader);

            return nearestResults;
        }

        public void UpdateClusterId(int recipeId, int clusterId)
        {
            EnsureConnectionOpened();

            var command = new CacheCommand(GetFullProcedureName("SitePage_UpdateClusterId"), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("PageId", recipeId);
            command.Parameters.Add("ClusterId", clusterId);

            command.ExecuteNonQuery();
        }

        public void UpdateSimilarResults(int pageId, IList<int> results, IList<int> weigths)
        {
            EnsureConnectionOpened();

            var command = new CacheCommand(GetFullProcedureName("SitePage_UpdateNearestResults"), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("PageId", pageId);

            // Serialize to string to reduce the number of DB requests
            // TODO: Revisit
            var resultsWithWeights = results.Select((val, idx) =>
                String.Format("{0};{1}", val, weigths[idx].ToString(CultureInfo.InvariantCulture)));

            command.Parameters.Add("NearestResults", String.Join(" ", resultsWithWeights)); 

            command.ExecuteNonQuery();
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
