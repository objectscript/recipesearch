namespace RecipesSearch.DAL.SqlServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CrawledSites", "ConfigId", "dbo.Configs");
            DropIndex("dbo.CrawledSites", new[] { "ConfigId" });
            CreateTable(
                "dbo.SiteToCrawls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        URL = c.String(nullable: false),
                        ConfigId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Configs", t => t.ConfigId, cascadeDelete: true)
                .Index(t => t.ConfigId);
            
            DropTable("dbo.CrawledSites");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CrawledSites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        URL = c.String(nullable: false),
                        ConfigId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.SiteToCrawls", "ConfigId", "dbo.Configs");
            DropIndex("dbo.SiteToCrawls", new[] { "ConfigId" });
            DropTable("dbo.SiteToCrawls");
            CreateIndex("dbo.CrawledSites", "ConfigId");
            AddForeignKey("dbo.CrawledSites", "ConfigId", "dbo.Configs", "Id", cascadeDelete: true);
        }
    }
}
