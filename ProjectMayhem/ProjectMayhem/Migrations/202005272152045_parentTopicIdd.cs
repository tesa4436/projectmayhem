namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class parentTopicIdd : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Topics", new[] { "ParentTopicId" });
            AlterColumn("dbo.Topics", "ParentTopicId", c => c.Int());
            CreateIndex("dbo.Topics", "ParentTopicId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Topics", new[] { "ParentTopicId" });
            AlterColumn("dbo.Topics", "ParentTopicId", c => c.Int(nullable: false));
            CreateIndex("dbo.Topics", "ParentTopicId");
        }
    }
}
