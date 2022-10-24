namespace projeArayuz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class durationString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Data", "duration", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Data", "duration", c => c.Int(nullable: false));
        }
    }
}
