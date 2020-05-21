namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrimaryKeys1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LearningDays", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LearningDays", "Title");
        }
    }
}
