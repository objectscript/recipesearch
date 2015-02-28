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
using RecipesSearch.Data.Models.Logging;
using RecipesSearch.Data.Views;

namespace RecipesSearch.DAL.Cache.Adapters
{
    public class CrawlingHistoryAdapter : CacheAdapter
    {
        public bool ClearAll()
        {
            var command = new CacheCommand(GetFullProcedureName("CrawlingHistoryItem_ClearAllHistory"), CacheConnection);
            command.CommandType = CommandType.StoredProcedure;

            return command.ExecuteNonQuery() != 0;
        }
    }
}
