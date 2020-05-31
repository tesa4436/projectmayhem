namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TopicUpdate2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Topics", name: "ParentID", newName: "parentTopic_TopicsId");
            RenameIndex(table: "dbo.Topics", name: "IX_ParentID", newName: "IX_parentTopic_TopicsId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Topics", name: "IX_parentTopic_TopicsId", newName: "IX_ParentID");
            RenameColumn(table: "dbo.Topics", name: "parentTopic_TopicsId", newName: "ParentID");
        }
    }
}
