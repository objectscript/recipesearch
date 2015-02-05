using System.Data.Entity;
using RecipesSearch.Data.Models;
using RecipesSearch.Data.Models.Logging;

namespace RecipesSearch.DAL.SqlServer.DatabaseContexts
{
    public class DatabaseContext : DbContext
    {
        public DbSet<LogRecord> LogRecords { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<SiteToCrawl> SitesToCrawl { get; set; }
        public DbSet<CrawlingHistoryItem> CrawlingHistory { get; set; }

        public DatabaseContext()
            : base("SiteData")
        {
        }
    }
}