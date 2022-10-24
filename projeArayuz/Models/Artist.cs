using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projeArayuz.Models
{
    internal class Artist
    {
        [Key]
        public int ArtistID { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string picture_medium { get; set; }
        public string type { get; set; }
    }
}
