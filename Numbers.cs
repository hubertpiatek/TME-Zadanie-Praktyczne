using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME_Zadanie_Praktyczne
{
    class Numbers
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        
        public int Value { get; set; }
        public String Status { get; set; }

        public Numbers(int id,int Value) {
            this.Id = id;
            this.Value = Value;
            this.Status = "false";
            
        }
    }
}
