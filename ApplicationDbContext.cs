using System;
using System.Data.Entity;

namespace TME_Zadanie_Praktyczne
{
    class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() :base(){}
        public DbSet<Numbers> Numbers { get; set; }

        public Guid Random()
        {
            return Guid.NewGuid();
        }
    }
}
