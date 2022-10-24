using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projeArayuz.Models
{
    internal class BaseClass
    {
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ModifiedDate { get; set; }

        public int CreatedBy { get; set; } 

        public int ModifiedBy { get; set; }

        public bool IsModified { get; set; }
    }
}
