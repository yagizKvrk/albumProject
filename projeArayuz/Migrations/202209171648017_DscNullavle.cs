namespace projeArayuz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DscNullavle : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShopInfoes", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShopInfoes", "Discount", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
