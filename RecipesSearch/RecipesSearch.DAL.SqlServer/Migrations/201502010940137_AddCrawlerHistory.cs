namespace RecipesSearch.DAL.SqlServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCrawlerHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CrawlingHistoryItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(nullable: false),
                        StardDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CrawledPagesCount = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SiteToCrawls", t => t.SiteId, cascadeDelete: false)
                .Index(t => t.SiteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CrawlingHistoryItems", "SiteId", "dbo.SiteToCrawls");
            DropIndex("dbo.CrawlingHistoryItems", new[] { "SiteId" });
            DropTable("dbo.CrawlingHistoryItems");
        }
    }
}
