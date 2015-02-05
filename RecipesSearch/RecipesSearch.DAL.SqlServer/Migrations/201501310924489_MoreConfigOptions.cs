namespace RecipesSearch.DAL.SqlServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreConfigOptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Configs", "LoggingEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Configs", "MaxPagesToCrawl", c => c.Int(nullable: false));
            AddColumn("dbo.Configs", "MaxCrawlDepth", c => c.Int(nullable: false));
            AddColumn("dbo.Configs", "CrawlTimeoutSeconds", c => c.Int(nullable: false));
            DropColumn("dbo.Configs", "MinutesToRestartCrawling");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Configs", "MinutesToRestartCrawling", c => c.Int(nullable: false));
            DropColumn("dbo.Configs", "CrawlTimeoutSeconds");
            DropColumn("dbo.Configs", "MaxCrawlDepth");
            DropColumn("dbo.Configs", "MaxPagesToCrawl");
            DropColumn("dbo.Configs", "LoggingEnabled");
        }
    }
}
