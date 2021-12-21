using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace TME_Zadanie_Praktyczne
{
    class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() :base(){}
        public DbSet<Numbers> Numbers { get; set; }

    }
}
