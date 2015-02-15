namespace RecipesSearch.DAL.SqlServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedKeywordProcessingSettingForCrawlerConfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Configs", "EnhancedKeywordProcessing", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Configs", "EnhancedKeywordProcessing");
        }
    }
}
