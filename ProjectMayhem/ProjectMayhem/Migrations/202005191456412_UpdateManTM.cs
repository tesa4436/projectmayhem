namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateManTM : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LDayReferences",
                c => new
                    {
                        LDayReferencesId = c.Int(nullable: false, identity: true),
                        ReferenceUrl = c.String(),
                        learningDay_LearningDayId = c.Int(),
                    })
                .PrimaryKey(t => t.LDayReferencesId)
                .ForeignKey("dbo.LearningDays", t => t.learningDay_LearningDayId)
                .Index(t => t.learningDay_LearningDayId);
            
            CreateTable(
                "dbo.LearningDays",
                c => new
                    {
                        LearningDayId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.LearningDayId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.TopicDays",
                c => new
                    {
                        TopicDayId = c.Int(nullable: false, identity: true),
                        Day_LearningDayId = c.Int(),
                        Topic_TopicsId = c.Int(),
                    })
                .PrimaryKey(t => t.TopicDayId)
                .ForeignKey("dbo.LearningDays", t => t.Day_LearningDayId)
                .ForeignKey("dbo.Topics", t => t.Topic_TopicsId)
                .Index(t => t.Day_LearningDayId)
                .Index(t => t.Topic_TopicsId);
            
            CreateTable(
                "dbo.Topics",
                c => new
                    {
                        TopicsId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.TopicsId);
            
            AddColumn("dbo.AspNetUsers", "teamLead_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AspNetUsers", "teamLead_Id");
            AddForeignKey("dbo.AspNetUsers", "teamLead_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "teamLead_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.LearningDays", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TopicDays", "Topic_TopicsId", "dbo.Topics");
            DropForeignKey("dbo.TopicDays", "Day_LearningDayId", "dbo.LearningDays");
            DropForeignKey("dbo.LDayReferences", "learningDay_LearningDayId", "dbo.LearningDays");
            DropIndex("dbo.AspNetUsers", new[] { "teamLead_Id" });
            DropIndex("dbo.TopicDays", new[] { "Topic_TopicsId" });
            DropIndex("dbo.TopicDays", new[] { "Day_LearningDayId" });
            DropIndex("dbo.LearningDays", new[] { "User_Id" });
            DropIndex("dbo.LDayReferences", new[] { "learningDay_LearningDayId" });
            DropColumn("dbo.AspNetUsers", "teamLead_Id");
            DropTable("dbo.Topics");
            DropTable("dbo.TopicDays");
            DropTable("dbo.LearningDays");
            DropTable("dbo.LDayReferences");
        }
    }
}
