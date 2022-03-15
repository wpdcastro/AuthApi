using FanPush.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ZAuth.Business.Interface;
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
        [Route("login")]
        public async Task<ActionResult<dynamic>> AuthenticateAsync([FromBody] User model)
        {
            try
            {
                model.Password = StringUtils.GerarHashMd5(model.Password);

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

                var token = TokenService.GenerateToken(user.Result);
                var refreshToken = TokenService.GenerateRefreshToken();
                TokenService.SaveRefreshToken(user.Result.Email, refreshToken);

                user.Result.Password = "";
                user.Result.Documento = "";

                return new
                {
                    user = user.Result,
                    token = token,
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

        [HttpPost]
        [Route("validar")]
        public async Task<ActionResult<dynamic>> AuthenticateAsyncValid([FromBody] User model)
        {
            try
            {
                model.Password = StringUtils.GerarHashMd5(model.Password);

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

                var token = TokenService.GenerateToken(user.Result);
                var refreshToken = TokenService.GenerateRefreshToken();
                TokenService.SaveRefreshToken(user.Result.Email, refreshToken);

                user.Result.Password = "";
                user.Result.Documento = "";

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
        [Route("refresh")]
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
