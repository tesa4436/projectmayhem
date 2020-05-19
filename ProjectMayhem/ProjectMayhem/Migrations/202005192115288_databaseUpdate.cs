namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class databaseUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Topics", "parentTopic_TopicsId", c => c.Int());
            CreateIndex("dbo.Topics", "parentTopic_TopicsId");
            AddForeignKey("dbo.Topics", "parentTopic_TopicsId", "dbo.Topics", "TopicsId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Topics", "parentTopic_TopicsId", "dbo.Topics");
            DropIndex("dbo.Topics", new[] { "parentTopic_TopicsId" });
            DropColumn("dbo.Topics", "parentTopic_TopicsId");
        }
    }
}
