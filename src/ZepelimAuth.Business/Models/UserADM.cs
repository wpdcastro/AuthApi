using System;
using System.Collections.Generic;

namespace ZepelimAuth.Business.Models
{
    public class UserADM : Entity
    {
        public string Nome { get; set; }
        public string Password { get; set; }
        public string DocumentoUsuario { get; set; }
        public string RazaoSocial { get; set; }
        public string DocumentoEmpresa { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime DataCadastro { get; set; }
        public List<string> EmpresasAtivas { get; set; }
        public List<ProdutosADM> Produtos { get; set; }
        public bool POS { get; set; }
        public bool HUB { get; set; }
        public bool LOG { get; set; }
    }
}
