namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class learningDayFix2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LDayReferences", "LDId", "dbo.LearningDays");
            DropForeignKey("dbo.TopicDays", "DayId", "dbo.LearningDays");
            DropForeignKey("dbo.LearningDays", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.LDayReferences", new[] { "LDId" });
            DropIndex("dbo.LearningDays", new[] { "UserId" });
            DropIndex("dbo.TopicDays", new[] { "DayId" });
            RenameColumn(table: "dbo.LDayReferences", name: "LDId", newName: "LearningDayId");
            RenameColumn(table: "dbo.TopicDays", name: "DayId", newName: "LearningDayId");
            DropPrimaryKey("dbo.LDayReferences");
            DropPrimaryKey("dbo.LearningDays");
            DropPrimaryKey("dbo.TopicDays");
            AddColumn("dbo.LDayReferences", "UserId", c => c.String(maxLength: 128));
            AddColumn("dbo.TopicDays", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.LearningDays", "UserId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.LDayReferences", "ReferenceUrl");
            AddPrimaryKey("dbo.LearningDays", new[] { "LearningDayId", "UserId" });
            AddPrimaryKey("dbo.TopicDays", new[] { "LearningDayId", "UserId", "TopicId" });
            CreateIndex("dbo.LDayReferences", new[] { "LearningDayId", "UserId" });
            CreateIndex("dbo.LearningDays", "UserId");
            CreateIndex("dbo.TopicDays", new[] { "LearningDayId", "UserId" });
            AddForeignKey("dbo.LDayReferences", new[] { "LearningDayId", "UserId" }, "dbo.LearningDays", new[] { "LearningDayId", "UserId" });
            AddForeignKey("dbo.TopicDays", new[] { "LearningDayId", "UserId" }, "dbo.LearningDays", new[] { "LearningDayId", "UserId" }, cascadeDelete: true);
            AddForeignKey("dbo.LearningDays", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LearningDays", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TopicDays", new[] { "LearningDayId", "UserId" }, "dbo.LearningDays");
            DropForeignKey("dbo.LDayReferences", new[] { "LearningDayId", "UserId" }, "dbo.LearningDays");
            DropIndex("dbo.TopicDays", new[] { "LearningDayId", "UserId" });
            DropIndex("dbo.LearningDays", new[] { "UserId" });
            DropIndex("dbo.LDayReferences", new[] { "LearningDayId", "UserId" });
            DropPrimaryKey("dbo.TopicDays");
            DropPrimaryKey("dbo.LearningDays");
            DropPrimaryKey("dbo.LDayReferences");
            AlterColumn("dbo.LearningDays", "UserId", c => c.String(maxLength: 128));
            DropColumn("dbo.TopicDays", "UserId");
            DropColumn("dbo.LDayReferences", "UserId");
            AddPrimaryKey("dbo.TopicDays", new[] { "DayId", "TopicId" });
            AddPrimaryKey("dbo.LearningDays", "LearningDayId");
            AddPrimaryKey("dbo.LDayReferences", new[] { "ReferenceUrl", "LDId" });
            RenameColumn(table: "dbo.TopicDays", name: "LearningDayId", newName: "DayId");
            RenameColumn(table: "dbo.LDayReferences", name: "LearningDayId", newName: "LDId");
            CreateIndex("dbo.TopicDays", "DayId");
            CreateIndex("dbo.LearningDays", "UserId");
            CreateIndex("dbo.LDayReferences", "LDId");
            AddForeignKey("dbo.LearningDays", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.TopicDays", "DayId", "dbo.LearningDays", "LearningDayId", cascadeDelete: true);
            AddForeignKey("dbo.LDayReferences", "LDId", "dbo.LearningDays", "LearningDayId", cascadeDelete: true);
        }
    }
}
