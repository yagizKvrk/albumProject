namespace projeArayuz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShopInfoes", "IsModified", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShopInfoes", "IsModified");
        }
    }
}
