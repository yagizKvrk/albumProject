namespace projeArayuz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manager : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Managers",
                c => new
                    {
                        ManagerId = c.Int(nullable: false, identity: true),
                        ManagerName = c.String(),
                        ManagerPassword = c.String(),
                    })
                .PrimaryKey(t => t.ManagerId);
            
            AddColumn("dbo.ShopInfoes", "ManagerId", c => c.Int(nullable: false));
            CreateIndex("dbo.ShopInfoes", "ManagerId");
            AddForeignKey("dbo.ShopInfoes", "ManagerId", "dbo.Managers", "ManagerId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShopInfoes", "ManagerId", "dbo.Managers");
            DropIndex("dbo.ShopInfoes", new[] { "ManagerId" });
            DropColumn("dbo.ShopInfoes", "ManagerId");
            DropTable("dbo.Managers");
        }
    }
}
