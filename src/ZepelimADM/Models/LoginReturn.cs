using System;
using System.Collections.Generic;

namespace ZepelimADM.Models
{
    public class LoginReturn
    {
        public int code { get; set; }
        public bool success { get; set; }
        public DateTime return_date { get; set; }
        public string message { get; set; }
        public UserLoginADM data { get; set; }
    }

    public  class UserLoginADM
    {
        public int id { get; set; }
        public Guid guid { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public int? usuarioLogId { get; set; }
        public int? empresaAtivaId { get; set; }
        public string role { get; set; }
        public string documento { get; set; }
        public virtual List<EmpresaAtivaADM> empresasAtivas { get; set; }
        public virtual List<AcessoPaginaADM> acessoPaginas  { get; set; }
        public virtual List<ProductoADM> produtos { get; set; }
    }

    public class ProductoADM
    {
        public string descricao { get; set; }
        public string connectionString { get; set; }
    }

    public class AcessoPaginaADM
    {

    }

    public class EmpresaAtivaADM
    {
        public int id { get; set; }
        public Guid guid { get; set; }
        public string descricao { get; set; }
        public string documento { get; set; }
    }
}
