using FanPush.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ZAuth.Business.Interface;
using ZepelimADM.Connections;
using ZepelimAuth.Business.Models;
using ZepelimAuth.Controllers.Utils;

namespace ZepelimAuth.Api.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public LoginController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public string ConnectionCliente
        {
            get
            {
                var identity = (ClaimsIdentity)User.Identity;

                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var res = identity.FindFirst("connetion");
                    if (res == null)
                    {
                        return null;
                    }
                    // or
                    string ci = identity.FindFirst("connetion").Value;
                    return ci;
                }
                return null;
            }
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] User model)
        {
            try
            {
                var connector     = new ZepelimADMConnector();
                var usuarioLogado = connector.Logar(model.Email, model.Senha);

                if (usuarioLogado == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Usuário ou senha inválidos"
                    });
                }
                // usuarioLogado
                var token = TokenService.GenerateToken(new User());
                var refreshToken = TokenService.GenerateRefreshToken();

                // Email
                TokenService.SaveRefreshToken(usuarioLogado.id.ToString() , refreshToken);

                connector.AlterarToken(usuarioLogado.id, token, refreshToken);

                var res = new
                {
                    user = usuarioLogado,
                    token = token,
                    refreshToken = refreshToken
                };

                return Ok(new
                {
                    code = 400,
                    return_date = DateTime.Now,
                    success = false,
                    data = res
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 400,
                    return_date = DateTime.Now,
                    success = false,
                    message = e.Message
                });
            }
        }

        /*
        [HttpPost]
        [Route("Login1")]
        public async Task<ActionResult<dynamic>> AuthenticateAsync([FromBody] User model)
        {
            try
            {
                model.Senha = StringUtils.GerarHashMd5(model.Password);

                var user = _userRepository.Get2(model.Email, model.Password);

                if (user.Result == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Usuário ou senha inválidos"
                    });
                }

                var connector     = new ZepelimADMConnector();
                UserADM usuarioLogado = connector.Login(model.Email, model.Password);

                if (usuarioLogado == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Usuário ou senha inválidos"
                    });
                }

                var produtosInvalidos = usuarioLogado.Produtos.Where(pr => pr.ConnectionString == null).ToList();

                if (produtosInvalidos.Count() > 0) {
                    connector.CriaBancos(usuarioLogado);
                }

                var token = TokenService.GenerateToken(usuarioLogado);
                var refreshToken = TokenService.GenerateRefreshToken();
                TokenService.SaveRefreshToken(user.Result.Email, refreshToken);

                usuarioLogado.Produtos = null;

                return new
                {
                    user         = usuarioLogado,
                    token        = token,
                    refreshToken = refreshToken
                };
            }
            catch(Exception e)
            {
                return BadRequest(new
                {
                    code = 400,
                    return_date = DateTime.Now,
                    success = false,
                    message = e.Message
                });
            }
        }
        */

        [HttpPost]
        [Route("Validar")]
        public async Task<ActionResult<dynamic>> AuthenticateAsyncValid([FromBody] User model)
        {
            try
            {
                model.Senha = StringUtils.GerarHashMd5(model.Senha);

                var user = _userRepository.Get2(model.Email, model.Senha);

                if (user.Result == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Usuário ou senha inválidos"
                    });
                }

                var token = TokenService.GenerateToken(user.Result);
                var refreshToken = TokenService.GenerateRefreshToken();
                TokenService.SaveRefreshToken(user.Result.Email, refreshToken);

                user.Result.Senha = "";
                user.Result.DocumentoUsuario = "";

                return new
                {
                    user = user.Result,
                    token = token,
                    refreshToken = refreshToken
                };
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 400,
                    return_date = DateTime.Now,
                    success = false,
                    message = e.Message
                });
            }
        }

        [HttpPost]
        [Route("Refresh")]
        public IActionResult Refresh(string token, string refreshToken)
        {
            var principal = TokenService.GetPrincipalFromExpiredToken(token);
            var email = principal.Identity.Name;

            // TO DO validar se o token está expirado ou não

            var savedRefreshToken = TokenService.GetRefreshToken(email);
            if (savedRefreshToken != refreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var newJtwToken = TokenService.GenerateToken(principal.Claims);
            var newRefreshToken = TokenService.GenerateRefreshToken();
            TokenService.DeleteRefreshToken(email, refreshToken);
            TokenService.SaveRefreshToken(email, newRefreshToken);

            return new ObjectResult(new
            {
                token = newJtwToken,
                refreshToken = newRefreshToken
            });
        }
    }
}
