using System;
using RecipesSearch.DAL.Cache.Adapters.Base;
using RecipesSearch.Data.Models.Logging;

namespace RecipesSearch.BusinessServices.Logging
{
    public static class Logger
    {
        private readonly static CacheAdapter CacheAdapter;
        private static object _lock = new {};

        static Logger()
        {
            // Maintain one app connection per AppDomain for logger
            CacheAdapter = new CacheAdapter();
        }

        public static void LogError(string description, Exception exception)
        {
            LogRecord(description, exception, LogRecordType.Error);
        }
        public static void LogWarning(string description, Exception exception)
        {
            LogRecord(description,exception,LogRecordType.Warning);
        }

        public static void LogCrawlerInfo(string info)
        {
            LogRecord(info, null, LogRecordType.CrawlerInfo);
        }

        private static void LogRecord(string description, Exception exception, LogRecordType type)
        {
            description = description ?? exception.ToString();

            var logRecord = new LogRecord
            {
                Description = description,
                Exception = exception != null ? exception.ToString() : null,
                CreatedDate = DateTime.UtcNow,
                Type = type
            };

            lock (_lock)
            {
                CacheAdapter.InsertEntity(logRecord);    
            }                 
        }
    }
}
