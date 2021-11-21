using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCalculation.Model
{
    public class Peserta
    {
        public string ID_PESERTA { get; set; }
        public string NAMA_PESERTA { get; set; }
        public string JENIS_KELAMIN { get; set; }
        public string TEMPAT_LAHIR { get; set; }
        public string TANGGAL_LAHIR { get; set; }
        public string ALAMAT_PESERTA { get; set; }
        public string SEKOLAH_PESERTA { get; set; }
        public string NO_TELP_PESERTA { get; set; }
        public string EMAIL_PESERTA { get; set; }
        public string IS_USMAS { get; set; }
        public string PASSWORD_PESERTA { get; set; }
        public string CABANG_CODE { get; set; }
        public string ENTRY_USER { get; set; }
        public string ENTRY_DATE { get; set; }
        public string UPDATE_USER { get; set; }
        public string UPDATE_DATE { get; set; }
    }
}
