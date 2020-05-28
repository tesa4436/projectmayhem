namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTopicLearningStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TopicUsers", "IsTopicLearned", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            
        }
    }
}
