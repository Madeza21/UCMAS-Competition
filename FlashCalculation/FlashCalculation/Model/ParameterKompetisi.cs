using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCalculation.Model
{
    public class ParameterKompetisi
    {
        public string ROW_ID_KOMPETISI { get; set; }
        public string PARAMETER_ID { get; set; }
        public string SOAL_DARI { get; set; }
        public string SOAL_SAMPAI { get; set; }
        public string PANJANG_DIGIT { get; set; }
        public string JUMLAH_MUNCUL { get; set; }
        public string JML_BARIS_PER_MUNCUL { get; set; }
        public string MAX_PANJANG_DIGIT { get; set; }
        public string MAX_JML_DIGIT_PER_SOAL { get; set; }
        public string JML_BARIS_PER_SOAL { get; set; }
        public string MUNCUL_ANGKA_MINUS { get; set; }
        public string MUNCUL_ANGKA_PERKALIAN { get; set; }
        public string DIGIT_PERKALIAN { get; set; }
        public string MUNCUL_ANGKA_PEMBAGIAN { get; set; }
        public string DIGIT_PEMBAGIAN { get; set; }
        public string MUNCUL_ANGKA_DECIMAL { get; set; }
        public string DIGIT_DECIMAL { get; set; }
        public string FONT_SIZE { get; set; }
        public string KECEPATAN { get; set; }
        public string ENTRY_USER { get; set; }
        public string ENTRY_DATE { get; set; }
        public string UPDATE_USER { get; set; }
        public string UPDATE_DATE { get; set; }
    }
}
