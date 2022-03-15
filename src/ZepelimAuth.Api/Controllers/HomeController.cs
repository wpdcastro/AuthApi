using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZepelimAuth.Api.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("anonymous")]
        [AllowAnonymous]
        public string Anonymous() => "Anônimo";

        [HttpGet]
        [Route("authenticated")]
        [Authorize]
        public string Authenticated() => $"Autenticado - {User.Identity.Name}";

        [HttpGet]
        [Route("admin")]
        [Authorize(Roles = "ADM")]
        public string Admin() => "Administrador";

        [HttpGet]
        [Route("fan")]
        [Authorize(Roles = "FAN")]
        public string Fan() => "Fã";

        [HttpGet]
        [Route("general")]
        [Authorize(Roles = "ADM,FAN")]
        public string General() => "Geral";
    }
}
