using System;
using NLog;
using RecipesSearch.DAL.Cache.Adapters.Base;
using RecipesSearch.Data.Models.Logging;
using System.Diagnostics;

namespace RecipesSearch.BusinessServices.Logging
{
    public static class LoggerWrapper
    {
        private static object _lock = new {};
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static void LogError(string description, Exception exception)
        {
            LogCacheRecord(description, exception, LogRecordType.Error);
            _logger.Fatal("Message = {0}. Exception = {1}", description, exception);
        }

        public static void LogInfo(string description)
        {
            LogCacheRecord(description, null, LogRecordType.Info);
            _logger.Info(description);
        }

        public static void LogWarning(string description, Exception exception)
        {
            LogCacheRecord(description,exception,LogRecordType.Warning);
            _logger.Warn(exception, description);
        }

        public static void LogCrawlerInfo(string info)
        {
            LogCacheRecord(info, null, LogRecordType.CrawlerInfo);
            _logger.Trace(info);
        }

        public static void LogActionTime(Action action, string actionName)
        {
            _logger.Trace(String.Format("{0}. Start.", actionName));
            Stopwatch sw = new Stopwatch();
            sw.Start();

            action();

            sw.Stop();
            _logger.Trace(String.Format("{0}. End. Elapsed milliseconds: {1}", actionName, sw.ElapsedMilliseconds));
        }

        public static T LogActionTime<T>(Func<T> action, string actionName)
        {
            _logger.Trace(String.Format("{0}. Start.", actionName));
            Stopwatch sw = new Stopwatch();
            sw.Start();

            T result = action();

            sw.Stop();
            _logger.Trace(String.Format("{0}. End. Elapsed milliseconds: {1}", actionName, sw.ElapsedMilliseconds));

            return result;
        }

        private static void LogCacheRecord(string description, Exception exception, LogRecordType type)
        {
            try
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
                    new CacheAdapter().InsertEntity(logRecord);
                }  
            }
            catch (Exception e)
            {
                _logger.Warn( e, "Exception during logging to Cache. Exception = {0}");
            }
                           
        }
    }
}
