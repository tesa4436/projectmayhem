namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixingMerge : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Topics",
                c => new
                    {
                        TopicID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Url = c.String(),
                        Description = c.String(),
                        ParentID_TopicID = c.Int(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.TopicID)
                .ForeignKey("dbo.Topics", t => t.ParentID_TopicID)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ParentID_TopicID)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Topics", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Topics", "ParentID_TopicID", "dbo.Topics");
            DropIndex("dbo.Topics", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Topics", new[] { "ParentID_TopicID" });
            DropTable("dbo.Topics");
        }
    }
}
