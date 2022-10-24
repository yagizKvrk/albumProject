namespace projeArayuz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class releaseDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Albums", "release_date", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Albums", "release_date");
        }
    }
}
