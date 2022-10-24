namespace projeArayuz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class email : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Managers", "MailAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Managers", "MailAddress");
        }
    }
}
