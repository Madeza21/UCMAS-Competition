using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCalculation.Model
{
    public class Kompetisi
    {
        public string ROW_ID { get; set; }
        public string CABANG_CODE { get; set; }
        public string KOMPETISI_NAME { get; set; }
        public string TANGGAL_KOMPETISI { get; set; }
        public string TANGGAL_SELESAI_TRIAL { get; set; }
        public string JAM_MULAI { get; set; }
        public string JAM_SAMPAI { get; set; }
        public string JENIS_CODE { get; set; }
        public string JENIS_NAME { get; set; }
        public string TIPE { get; set; }
        public string ROW_ID_KATEGORI { get; set; }
        public string KATEGORI_CODE { get; set; }
        public string KATEGORI_NAME { get; set; }
        public string LAMA_PERLOMBAAN { get; set; }
        public string KECEPATAN { get; set; }
        public string ENTRY_USER { get; set; }
        public string ENTRY_DATE { get; set; }
        public string UPDATE_USER { get; set; }
        public string UPDATE_DATE { get; set; }
        public string BAHASA { get; set; }
        public string FLAG { get; set; }
    }
}
