using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCalculation.Model
{
    public class JawabanKompetisi
    {
        public string ROW_ID_KOMPETISI { get; set; }
        public string ID_PESERTA { get; set; }
        public int SOAL_NO { get; set; }
        public string PERTANYAAN { get; set; }
        public decimal JAWABAN_PESERTA { get; set; }
        public int JAWAB_DETIK_BERAPA { get; set; }
        public DateTime JAWAB_DATE { get; set; }
        public decimal KUNCI_JAWABAN { get; set; }
        public int SCORE_PESERTA { get; set; }
        public string ENTRY_USER { get; set; }
        public DateTime ENTRY_DATE { get; set; }
        public string UPDATE_USER { get; set; }
        public DateTime UPDATE_DATE { get; set; }
    }
}
