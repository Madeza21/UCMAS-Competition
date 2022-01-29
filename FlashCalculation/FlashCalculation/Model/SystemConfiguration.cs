using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCalculation.Model
{
    public class SystemConfiguration
    {
        public int ID { get; set; }
        public int START_DATE { get; set; }
        public int END_DATE { get; set; }
        public string SHOW_SPEECH_IND { get; set; }
        public string APP_VERSION { get; set; }
    }
}
