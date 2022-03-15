using System;

namespace ZepelimAuth.Business.Models
{
    public class User : Entity
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Documento { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool POS { get; set; }
        public bool HUB { get; set; }
        public bool LOG { get; set; }
    }
}
