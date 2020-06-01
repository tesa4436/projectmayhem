namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class completeRedo : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Topics", name: "parentTopic_TopicsId", newName: "ParentTopicId");
            RenameIndex(table: "dbo.Topics", name: "IX_parentTopic_TopicsId", newName: "IX_ParentTopicId");
            DropPrimaryKey("dbo.LDayReferences");
            //AddColumn("dbo.LDayReferences", "ReferenceId", c => c.Int(nullable: false, identity: true));
            //AddColumn("dbo.TopicUsers", "IsTopicLearned", c => c.Boolean(nullable: false));
            AlterColumn("dbo.LDayReferences", "ReferenceUrl", c => c.String());
            AddPrimaryKey("dbo.LDayReferences", new[] { "LearningDayId", "UserId", "ReferenceId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.LDayReferences");
            AlterColumn("dbo.LDayReferences", "ReferenceUrl", c => c.String(nullable: false, maxLength: 128));
            //DropColumn("dbo.TopicUsers", "IsTopicLearned");
            //DropColumn("dbo.LDayReferences", "ReferenceId");
            AddPrimaryKey("dbo.LDayReferences", new[] { "LearningDayId", "UserId", "ReferenceUrl" });
            RenameIndex(table: "dbo.Topics", name: "IX_ParentTopicId", newName: "IX_parentTopic_TopicsId");
            RenameColumn(table: "dbo.Topics", name: "ParentTopicId", newName: "parentTopic_TopicsId");
        }
    }
}
