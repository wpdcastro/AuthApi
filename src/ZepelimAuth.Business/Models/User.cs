using System;

namespace ZepelimAuth.Business.Models
{
    public class User : Entity
    {
        public string Nome { get; set; }
        public string Senha { get; set; }
        public string DocumentoUsuario { get; set; }
        public string RazaoSocial { get; set; }
        public string DocumentoEmpresa { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string CodigoArea { get; set; }
        public string DDD { get; set; }
        public string Telefone { get; set; }
        public int? UsuarioLogId { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool POS { get; set; }
        public bool HUB { get; set; }
        public bool LOG { get; set; }
    }
}
