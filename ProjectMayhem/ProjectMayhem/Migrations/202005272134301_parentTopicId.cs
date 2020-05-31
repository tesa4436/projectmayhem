namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class parentTopicId : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Topics", new[] { "parentTopic_TopicsId" });
            RenameColumn(table: "dbo.Topics", name: "parentTopic_TopicsId", newName: "ParentTopicId");
            AlterColumn("dbo.Topics", "ParentTopicId", c => c.Int(nullable: false));
            CreateIndex("dbo.Topics", "ParentTopicId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Topics", new[] { "ParentTopicId" });
            AlterColumn("dbo.Topics", "ParentTopicId", c => c.Int());
            RenameColumn(table: "dbo.Topics", name: "ParentTopicId", newName: "parentTopic_TopicsId");
            CreateIndex("dbo.Topics", "parentTopic_TopicsId");
        }
    }
}
