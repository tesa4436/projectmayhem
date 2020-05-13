namespace ProjectMayhem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_Testing : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Testing", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Testing");
        }
    }
}
