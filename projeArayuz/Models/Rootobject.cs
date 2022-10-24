using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static projeArayuz.Form1;

namespace projeArayuz.Models
{
    internal class Rootobject
    {
        public string title { get; set; }
        public int duration { get; set; }
        public string cover_medium { get; set; }
        public List<Datum> data { get; set; }
        public int total { get; set; }
        public string release_date { get; set; }
        public List<Contributor> contributors { get; set; }
    }
}
