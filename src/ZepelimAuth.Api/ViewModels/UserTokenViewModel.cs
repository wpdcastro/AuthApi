namespace ZepelimAuth.Api.ViewModels
{
    public class UserTokenViewModel
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public int? EmpresaAtivaId { get; set; }
    }
}
