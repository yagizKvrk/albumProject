namespace projeArayuz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Albums",
                c => new
                    {
                        AlbumID = c.Int(nullable: false, identity: true),
                        id = c.Int(nullable: false),
                        title = c.String(),
                        cover_medium = c.String(),
                        tracklist = c.String(),
                        type = c.String(),
                    })
                .PrimaryKey(t => t.AlbumID);
            
            CreateTable(
                "dbo.Artists",
                c => new
                    {
                        ArtistID = c.Int(nullable: false, identity: true),
                        id = c.Int(nullable: false),
                        name = c.String(),
                        picture_medium = c.String(),
                        type = c.String(),
                    })
                .PrimaryKey(t => t.ArtistID);
            
            CreateTable(
                "dbo.Data",
                c => new
                    {
                        DatumID = c.Int(nullable: false, identity: true),
                        id = c.Int(nullable: false),
                        title = c.String(),
                        duration = c.Int(nullable: false),
                        preview = c.String(),
                        type = c.String(),
                        album_AlbumID = c.Int(),
                        artist_ArtistID = c.Int(),
                    })
                .PrimaryKey(t => t.DatumID)
                .ForeignKey("dbo.Albums", t => t.album_AlbumID)
                .ForeignKey("dbo.Artists", t => t.artist_ArtistID)
                .Index(t => t.album_AlbumID)
                .Index(t => t.artist_ArtistID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Data", "artist_ArtistID", "dbo.Artists");
            DropForeignKey("dbo.Data", "album_AlbumID", "dbo.Albums");
            DropIndex("dbo.Data", new[] { "artist_ArtistID" });
            DropIndex("dbo.Data", new[] { "album_AlbumID" });
            DropTable("dbo.Data");
            DropTable("dbo.Artists");
            DropTable("dbo.Albums");
        }
    }
}
