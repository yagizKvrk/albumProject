namespace projeArayuz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class baseclass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShopInfoes", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.ShopInfoes", "ModifiedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.ShopInfoes", "CreatedBy", c => c.Int(nullable: false));
            AddColumn("dbo.ShopInfoes", "ModifiedBy", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShopInfoes", "ModifiedBy");
            DropColumn("dbo.ShopInfoes", "CreatedBy");
            DropColumn("dbo.ShopInfoes", "ModifiedDate");
            DropColumn("dbo.ShopInfoes", "CreatedDate");
        }
    }
}
