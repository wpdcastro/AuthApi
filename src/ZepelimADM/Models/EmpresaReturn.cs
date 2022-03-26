using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZepelimADM.Models
{
    public class EmpresaReturn
    {
        public int code { get; set; }
        public bool success { get; set; }
        public DateTime return_date { get; set; }
        public string message { get; set; }
        public ObjectADM data { get; set; }
    }

    public  class ObjectADM
    {
        public int id { get; set; }
        public Guid guid { get; set; }
    }
}
