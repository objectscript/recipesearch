using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Policy;
using RecipesSearch.Data.Models;

namespace RecipesSearch.DAL.SqlServer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContexts.DatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DatabaseContexts.DatabaseContext context)
        {
            context.CrawlingHistory.RemoveRange(context.CrawlingHistory);
            context.SaveChanges();

            context.Configs.RemoveRange(context.Configs);
            context.Configs.Add(new Config
            {
                LoggingEnabled = true,
                CrawlTimeoutSeconds = 0,
                MaxCrawlDepth = 10000,
                MaxPagesToCrawl = 10000,
                SitesToCrawl = new List<SiteToCrawl>(new[]
                {
                    new SiteToCrawl {URL = "http://www.say7.info/", Name = "say7"},
                    new SiteToCrawl {URL = "http://www.povarenok.ru/", Name = "povarenok"},
                })
            });
        }
    }
}
