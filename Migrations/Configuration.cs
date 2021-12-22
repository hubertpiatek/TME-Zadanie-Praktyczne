namespace TME_Zadanie_Praktyczne.Migrations
{
    using System.Data.Entity.Migrations;


    internal sealed class Configuration : DbMigrationsConfiguration<TME_Zadanie_Praktyczne.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TME_Zadanie_Praktyczne.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
