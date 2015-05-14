using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterSystems.Data.CacheClient;
using RecipesSearch.DAL.Cache.Adapters.Base;
using RecipesSearch.Data.Framework;

namespace RecipesSearch.DAL.Cache.Adapters
{
    public class TfIdfAdapter :  CacheAdapter
    {
        public int GetTfIdfStatistic()
        {
            var command = new CacheCommand(GetFullProcedureName("SitePage_GetTFIDFStatistic"), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            var result = command.ExecuteScalar();

            return (int)result;
        }

        public void UpdateTfIdf()
        {
            RunTfIdfTask("SitePage_UpdateTfIdf", Constants.DefaultCachePackage);
        }

        public void UpdateTf(string builderName)
        {
            RunTfIdfTask("Builder_BuildTf", Constants.TfBuilderCachePackage, command =>
            {
                command.Parameters.AddWithValue("builderName", builderName);
            });
        }

        public void UpdateIdf(string builderName)
        {
            RunTfIdfTask("Builder_BuildIdf", Constants.IdfBuilderCachePackage, command =>
            {
                command.Parameters.AddWithValue("builderName", builderName);
            });
        }

        public List<string> GetTfBuilders()
        {
            return GetBuilders("Builder_GetTfBuilders", Constants.TfBuilderCachePackage);
        }

        public List<string> GetIdfBuilders()
        {
            return GetBuilders("Builder_GetIdfBuilders", Constants.IdfBuilderCachePackage);
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

        private void RunTfIdfTask(string sprocName, string packageName, Action<CacheCommand> addParameters = null)
        {
            var command = new CacheCommand(GetFullProcedureName(sprocName, packageName), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            if (addParameters != null)
            {
                addParameters(command);
            }            

            command.CommandTimeout = 60 * 60; // 1 hour

            command.ExecuteNonQuery();
        }
    }
}
