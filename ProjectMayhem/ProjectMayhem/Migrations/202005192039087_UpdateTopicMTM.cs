namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTopicMTM : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TopicDays", "Day_LearningDayId", "dbo.LearningDays");
            DropForeignKey("dbo.TopicDays", "Topic_TopicsId", "dbo.Topics");
            DropIndex("dbo.TopicDays", new[] { "Day_LearningDayId" });
            DropIndex("dbo.TopicDays", new[] { "Topic_TopicsId" });
            RenameColumn(table: "dbo.TopicDays", name: "Day_LearningDayId", newName: "DayId");
            RenameColumn(table: "dbo.TopicDays", name: "Topic_TopicsId", newName: "TopicId");
            DropPrimaryKey("dbo.TopicDays");
            AlterColumn("dbo.TopicDays", "DayId", c => c.Int(nullable: false));
            AlterColumn("dbo.TopicDays", "TopicId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.TopicDays", new[] { "DayId", "TopicId" });
            CreateIndex("dbo.TopicDays", "DayId");
            CreateIndex("dbo.TopicDays", "TopicId");
            AddForeignKey("dbo.TopicDays", "DayId", "dbo.LearningDays", "LearningDayId", cascadeDelete: true);
            AddForeignKey("dbo.TopicDays", "TopicId", "dbo.Topics", "TopicsId", cascadeDelete: true);
            DropColumn("dbo.TopicDays", "TopicDayId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TopicDays", "TopicDayId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.TopicDays", "TopicId", "dbo.Topics");
            DropForeignKey("dbo.TopicDays", "DayId", "dbo.LearningDays");
            DropIndex("dbo.TopicDays", new[] { "TopicId" });
            DropIndex("dbo.TopicDays", new[] { "DayId" });
            DropPrimaryKey("dbo.TopicDays");
            AlterColumn("dbo.TopicDays", "TopicId", c => c.Int());
            AlterColumn("dbo.TopicDays", "DayId", c => c.Int());
            AddPrimaryKey("dbo.TopicDays", "TopicDayId");
            RenameColumn(table: "dbo.TopicDays", name: "TopicId", newName: "Topic_TopicsId");
            RenameColumn(table: "dbo.TopicDays", name: "DayId", newName: "Day_LearningDayId");
            CreateIndex("dbo.TopicDays", "Topic_TopicsId");
            CreateIndex("dbo.TopicDays", "Day_LearningDayId");
            AddForeignKey("dbo.TopicDays", "Topic_TopicsId", "dbo.Topics", "TopicsId");
            AddForeignKey("dbo.TopicDays", "Day_LearningDayId", "dbo.LearningDays", "LearningDayId");
        }
    }
}
