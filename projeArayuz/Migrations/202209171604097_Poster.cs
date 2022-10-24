namespace projeArayuz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Poster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShopInfoes", "Poster", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShopInfoes", "Poster");
        }
    }
}
