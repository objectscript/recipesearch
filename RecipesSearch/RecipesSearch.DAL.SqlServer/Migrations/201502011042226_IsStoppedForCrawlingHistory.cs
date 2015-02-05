namespace RecipesSearch.DAL.SqlServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsStoppedForCrawlingHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CrawlingHistoryItems", "IsStopped", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CrawlingHistoryItems", "IsStopped");
        }
    }
}
