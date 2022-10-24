namespace projeArayuz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class son : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Albums", "duration", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Albums", "duration");
        }
    }
}
