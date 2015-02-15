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
using RecipesSearch.Data.Models;
using RecipesSearch.Data.Views;

namespace RecipesSearch.DAL.Cache
{
    public class CacheAdapter : IDisposable
    {
        private readonly CacheConnection _cacheConnection = new CacheConnection();

        public CacheAdapter()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Cache"].ConnectionString;
            _cacheConnection.ConnectionString = connectionString;
            _cacheConnection.Open();
        }

        public bool AddSitePage(SitePage sitePage)
        {
            var command = new CacheCommand("RecipesSearch.SitePage_Upsert", _cacheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("URL", sitePage.URL);
            command.Parameters.Add("Content", sitePage.Content);
            command.Parameters.Add("Keywords", sitePage.Keywords);

            var siteIdParemeter = new CacheParameter("SiteId", CacheDbType.Int);
            siteIdParemeter.Value = sitePage.SiteID;
            command.Parameters.Add(siteIdParemeter);

            return command.ExecuteNonQuery() != 0;
        }

        public List<SitePage> SearchByQuery(string searchQuery)
        {
            var command = new CacheCommand("RecipesSearch.SitePage_GetRecords", _cacheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("query", String.Format("%{0}%",searchQuery));

            var dataReader = command.ExecuteReader();

            var sitePages = ObjectMapper.Map<SitePage>(dataReader);

            return sitePages;
        }

        public List<SiteInfo> GetSitesInfo()
        {
            var command = new CacheCommand("RecipesSearch.SitePage_GetRecordsBySiteId", _cacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            var dataReader = command.ExecuteReader();

            var sitesInfo = ObjectMapper.Map<SiteInfo>(dataReader);

            return sitesInfo;
        }

        public bool DeleteSiteRecords(int siteId)
        {
            var command = new CacheCommand("RecipesSearch.SitePage_DeleteRecordsForSiteId", _cacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            var siteIdParemeter = new CacheParameter("SiteId", CacheDbType.Int);
            siteIdParemeter.Value = siteId;
            command.Parameters.Add(siteIdParemeter);

            return command.ExecuteNonQuery() != 0;
        }

        public bool DeleteSitesRecords()
        {
            var command = new CacheCommand("RecipesSearch.SitePage_DeleteSitesRecords", _cacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            return command.ExecuteNonQuery() != 0;
        }

        public void Dispose()
        {
            _cacheConnection.Close();
        }
    }
}
