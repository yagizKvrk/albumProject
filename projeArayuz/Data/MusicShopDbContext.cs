using projeArayuz.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projeArayuz.Data
{
    internal class MusicShopDbContext : DbContext
    {
        public MusicShopDbContext() : base("MusicShopDbContext")
        {
            
        }

        public DbSet<Datum> Tracks { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<ShopInfo> ShopInfos { get; set; }
        public DbSet<Manager> Managers { get; set; }
    }
}
