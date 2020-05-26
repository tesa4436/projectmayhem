namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class optlockForLD : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LearningDays", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LearningDays", "RowVersion");
        }
    }
}
