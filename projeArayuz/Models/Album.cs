using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projeArayuz.Models
{
    internal class Album
    {
        [Key]
        public int AlbumID { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public int duration { get; set; }
        public string release_date { get; set; }
        public string cover_medium { get; set; }
        public string tracklist { get; set; }
        public string type { get; set; }
    }
}
