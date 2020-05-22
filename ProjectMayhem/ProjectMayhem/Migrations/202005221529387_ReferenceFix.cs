namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferenceFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LDayReferences", new[] { "LearningDayId", "UserId" }, "dbo.LearningDays");
            DropIndex("dbo.LDayReferences", new[] { "LearningDayId", "UserId" });
            DropPrimaryKey("dbo.LDayReferences");
            AlterColumn("dbo.LDayReferences", "UserId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.LDayReferences", new[] { "LearningDayId", "UserId", "ReferenceUrl" });
            CreateIndex("dbo.LDayReferences", new[] { "LearningDayId", "UserId" });
            AddForeignKey("dbo.LDayReferences", new[] { "LearningDayId", "UserId" }, "dbo.LearningDays", new[] { "LearningDayId", "UserId" }, cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LDayReferences", new[] { "LearningDayId", "UserId" }, "dbo.LearningDays");
            DropIndex("dbo.LDayReferences", new[] { "LearningDayId", "UserId" });
            DropPrimaryKey("dbo.LDayReferences");
            AlterColumn("dbo.LDayReferences", "UserId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.LDayReferences", "ReferenceUrl");
            CreateIndex("dbo.LDayReferences", new[] { "LearningDayId", "UserId" });
            AddForeignKey("dbo.LDayReferences", new[] { "LearningDayId", "UserId" }, "dbo.LearningDays", new[] { "LearningDayId", "UserId" });
        }
    }
}
