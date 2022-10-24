using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projeArayuz.Models
{
    internal class ShopInfo : BaseClass
    {
        public int id { get; set; }
        public string AlbumTitle { get; set; }
        public string AlbumArtist { get; set; }
        public string ReleaseDate { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public bool StockStatus { get; set; }
        public string Poster { get; set; }

        public int ManagerId { get; set; }
        public Manager manager { get; set; }
    }
}
