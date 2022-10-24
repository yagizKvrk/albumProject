using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projeArayuz.Models
{
    internal class Datum
    {
        [Key]
        public int DatumID { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string duration { get; set; }
        public string preview { get; set; }
        public Artist artist { get; set; }
        public Album album { get; set; }
        public string type { get; set; }
    }
}
