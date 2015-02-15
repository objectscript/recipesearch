namespace RecipesSearch.DAL.SqlServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSearchSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SearchSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResultsOnPage = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SearchSettings");
        }
    }
}
