using System;
using RecipesSearch.DAL.SqlServer.DatabaseContexts;
using RecipesSearch.Data.Models.Logging;

namespace RecipesSearch.BusinessServices.Logging
{
    public static class Logger
    {
        private static readonly DatabaseContext DbContext = new DatabaseContext();            

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

            DbContext.LogRecords.Add(new LogRecord
            {
                Description = description,
                Exception = exception != null ? exception.ToString() : null,
                ExceptionStackTrace = exception != null ? exception.StackTrace : null,
                CreatedDate = DateTime.UtcNow,
                Type = type
            });
            DbContext.SaveChanges();
        }
    }
}
