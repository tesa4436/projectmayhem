namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class learningDayFix : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.LearningDays", name: "User_Id", newName: "UserId");
            RenameIndex(table: "dbo.LearningDays", name: "IX_User_Id", newName: "IX_UserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.LearningDays", name: "IX_UserId", newName: "IX_User_Id");
            RenameColumn(table: "dbo.LearningDays", name: "UserId", newName: "User_Id");
        }
    }
}
