using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InterSystems.Data.CacheClient;
using RecipesSearch.DAL.Cache.Adapters.Base;
using RecipesSearch.Data.Framework;

namespace RecipesSearch.DAL.Cache.Adapters
{
    public class TfIdfAdapter :  CacheAdapter
    {
        private int BatchSize { get { return 100; } }

        public int GetTfIdfStatistic()
        {
            return GetStatistic("SitePage_GetTFIDFStatistic", Constants.DefaultCachePackage);
        }

        public int GetTfStatistic()
        {
            return GetStatistic("SitePage_GetTfStatistic", Constants.DefaultCachePackage);
        }

        public int GetIdfStatistic()
        {
            return GetStatistic("SitePage_GetIdfStatistic", Constants.DefaultCachePackage);
        }

        public int GetCountOfRecipesWithEmptyCLusters()
        {
            return GetStatistic("SitePage_GetClustersStatistic", Constants.DefaultCachePackage);
        }

        public void UpdateTfIdf(CancellationToken cancellationToken, Action<decimal> progressCallback = null)
        {
            RunTfIdfTask("TfIdfBuilder_BuildTfIdfQuery", Constants.RecipeAnalyzeCachePackage, cancellationToken, null, progressCallback);
        }

        public void UpdateTf(string builderName, CancellationToken cancellationToken, Action<decimal> progressCallback = null)
        {
            RunTfIdfTask(
                "Builder_BuildTfQuery", 
                Constants.TfBuilderCachePackage, 
                cancellationToken,
                command => command.Parameters.AddWithValue("builderName", builderName),
                progressCallback
            );
        }

        public void UpdateIdf(string builderName)
        {
            var command = new CacheCommand(GetFullProcedureName("Builder_BuildIdf", Constants.IdfBuilderCachePackage), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("builderName", builderName);

            command.CommandTimeout = 60 * 10;

            command.ExecuteNonQuery();
        }

        public List<string> GetTfBuilders()
        {
            return GetBuilders("Builder_GetTfBuilders", Constants.TfBuilderCachePackage);
        }

        public List<string> GetIdfBuilders()
        {
            return GetBuilders("Builder_GetIdfBuilders", Constants.IdfBuilderCachePackage);
        }

        public List<string> GetOnlineTfIdfBuilders()
        {
            return GetBuilders("SearchSettings_GetOnlineTfIdfBuilders", Constants.DefaultCachePackage);
        }

        private int GetStatistic(string sprocName, string packageName)
        {
            var command = new CacheCommand(GetFullProcedureName(sprocName, packageName), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            var result = command.ExecuteScalar();

            if(result is Decimal)
            {
                return (int)(decimal)result;
            }

            return (int)result;
        }

        private List<string> GetBuilders(string sprocName, string packageName)
        {
            var command = new CacheCommand(GetFullProcedureName(sprocName, packageName), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            var dataReader = command.ExecuteReader();

            var result = new List<string>();

            while (dataReader.Read())
            {
                result.Add(dataReader[0].ToString());
            }

            return result;
        }

        private void RunTfIdfTask(string sprocName, string packageName, CancellationToken cancellationToken, Action<CacheCommand> addParameters = null, Action<decimal> progressCallback = null)
        {
            var recipesCount = GetRecipesCount();
            int updatedCount = 0;

            int idx = recipesCount.MinId;
            while (idx <= recipesCount.MaxId)
            {
                var command = new CacheCommand(GetFullProcedureName(sprocName, packageName), CacheConnection);
                command.CommandType = CommandType.StoredProcedure;

                if (addParameters != null)
                {
                    addParameters(command);
                }

                command.Parameters.AddWithValue("startId", idx);
                command.Parameters.AddWithValue("endId", idx + BatchSize - 1);

                command.CommandTimeout = 60;

                //var reader = command.ExecuteReader();
                updatedCount += (int)command.ExecuteScalar();

                idx += BatchSize;

                if (progressCallback != null)
                {
                    progressCallback(updatedCount*1m/recipesCount.Count*100m);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }       
        }

        private RecipesCount GetRecipesCount()
        {
            var command = new CacheCommand(GetFullProcedureName("SitePage_GetRecipesCount"), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                var recipesCount = new RecipesCount
                {
                    Count = (int) reader[0],
                    MinId = (int) reader[1],
                    MaxId = (int) reader[2],
                };
                return recipesCount;
            }
        }

        private struct RecipesCount
        {
            public int Count { get; set; }

            public int MinId { get; set; }

            public int MaxId { get; set; }
        }
    }
}
