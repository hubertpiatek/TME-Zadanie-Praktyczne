using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME_Zadanie_Praktyczne
{
    class ApplicationDbContext : DbContext
    {
        public DbSet<Numbers> Numbers { get; set; }
    }
}
