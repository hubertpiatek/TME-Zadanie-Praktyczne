namespace TME_Zadanie_Praktyczne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationForNumber : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Numbers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Int(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Numbers");
        }
    }
}
