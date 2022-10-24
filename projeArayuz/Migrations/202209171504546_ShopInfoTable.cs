namespace projeArayuz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShopInfoTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShopInfoes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        AlbumTitle = c.String(),
                        AlbumArtist = c.String(),
                        ReleaseDate = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Decimal(precision: 18, scale: 2),
                        StockStatus = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShopInfoes");
        }
    }
}
