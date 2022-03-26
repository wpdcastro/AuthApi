using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZepelimADM.Models
{
    public class EmpresaReturn12
    {
        public int code { get; set; }
        public bool success { get; set; }
        public DateTime return_date { get; set; }
        public dynamic message { get; set; }
        public dynamic data { get; set; }
    }

    public  class EmpresaADM2
    {
        public int id { get; set; }
        public string descricao { get; set; }
        public string documento { get; set; }
        public Guid guid { get; set; }
    }
}
