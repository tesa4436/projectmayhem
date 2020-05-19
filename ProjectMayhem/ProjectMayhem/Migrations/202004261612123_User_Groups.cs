namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_Groups : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "teamLead_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AspNetUsers", "teamLead_Id");
            AddForeignKey("dbo.AspNetUsers", "teamLead_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "teamLead_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUsers", new[] { "teamLead_Id" });
            DropColumn("dbo.AspNetUsers", "teamLead_Id");
        }
    }
}
