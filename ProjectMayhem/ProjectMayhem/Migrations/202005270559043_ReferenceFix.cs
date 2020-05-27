namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferenceFix : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.LDayReferences");
            AddColumn("dbo.LDayReferences", "ReferenceId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.LDayReferences", "ReferenceUrl", c => c.String());
            AddPrimaryKey("dbo.LDayReferences", new[] { "LearningDayId", "UserId", "ReferenceId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.LDayReferences");
            AlterColumn("dbo.LDayReferences", "ReferenceUrl", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.LDayReferences", "ReferenceId");
            AddPrimaryKey("dbo.LDayReferences", new[] { "LearningDayId", "UserId", "ReferenceUrl" });
        }
    }
}
