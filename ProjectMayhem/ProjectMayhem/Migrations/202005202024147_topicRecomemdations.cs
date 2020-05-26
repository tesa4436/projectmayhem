namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class topicRecomemdations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TopicUsers",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        TopicId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.TopicId })
                .ForeignKey("dbo.Topics", t => t.TopicId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.TopicId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TopicUsers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TopicUsers", "TopicId", "dbo.Topics");
            DropIndex("dbo.TopicUsers", new[] { "TopicId" });
            DropIndex("dbo.TopicUsers", new[] { "UserId" });
            DropTable("dbo.TopicUsers");
        }
    }
}
