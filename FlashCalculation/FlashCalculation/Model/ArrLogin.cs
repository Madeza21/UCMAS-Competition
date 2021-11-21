using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCalculation.Model
{
    public class ArrLogin 
    {
        public OutputLogin[] data { get; set; }
        public Peserta[] peserta { get; set; }
        public Kompetisi[] kompetisi { get; set; }
        public ParameterKompetisi[] parameterkompetisi { get; set; }
        public string Status { get; set; }
    }
    
}
