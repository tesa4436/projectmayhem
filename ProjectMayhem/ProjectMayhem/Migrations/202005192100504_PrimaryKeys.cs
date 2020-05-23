namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrimaryKeys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LDayReferences", "learningDay_LearningDayId", "dbo.LearningDays");
            DropIndex("dbo.LDayReferences", new[] { "learningDay_LearningDayId" });
            RenameColumn(table: "dbo.LDayReferences", name: "learningDay_LearningDayId", newName: "LDId");
            DropPrimaryKey("dbo.LDayReferences");
            AlterColumn("dbo.LDayReferences", "ReferenceUrl", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.LDayReferences", "LDId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.LDayReferences", new[] { "ReferenceUrl", "LDId" });
            CreateIndex("dbo.LDayReferences", "LDId");
            AddForeignKey("dbo.LDayReferences", "LDId", "dbo.LearningDays", "LearningDayId", cascadeDelete: true);
            DropColumn("dbo.LDayReferences", "LDayReferencesId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LDayReferences", "LDayReferencesId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.LDayReferences", "LDId", "dbo.LearningDays");
            DropIndex("dbo.LDayReferences", new[] { "LDId" });
            DropPrimaryKey("dbo.LDayReferences");
            AlterColumn("dbo.LDayReferences", "LDId", c => c.Int());
            AlterColumn("dbo.LDayReferences", "ReferenceUrl", c => c.String());
            AddPrimaryKey("dbo.LDayReferences", "LDayReferencesId");
            RenameColumn(table: "dbo.LDayReferences", name: "LDId", newName: "learningDay_LearningDayId");
            CreateIndex("dbo.LDayReferences", "learningDay_LearningDayId");
            AddForeignKey("dbo.LDayReferences", "learningDay_LearningDayId", "dbo.LearningDays", "LearningDayId");
        }
    }
}
