using FanPush.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ZAuth.Business.Interface;
using ZepelimAuth.Business.Models;
using ZepelimAuth.Controllers.Utils;
using ZepelimADM.Connections;
using ZepelimAuth.Api.ViewModels;

namespace ZepelimAuth.Api.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("salvar")]
        public IActionResult Save([FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Usuário não encontrado"
                    });
                }

                if (user.DocumentoUsuario == null)
                {
                    return BadRequest(new
                    {
                        code = 400,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Documento nulo ou inválido"
                    });
                }

                user.Role         = "ADMIN";
                user.DataCadastro = DateTime.UtcNow;

                var connector = new ZepelimADMConnector();
                var empresa = connector.CriarEmpresa(user.Id);
                if (empresa == null) throw new Exception("Erro ao salvar empresa");

                // connector.AtrelarEmpresaProduto(empresa.id, 8);
                // connector.AtrelarEmpresaProduto(empresa.id, 9);

                var usuario = connector.CriarUsuario(user);
                if (usuario == null) throw new Exception("Erro ao salvar usuário");
                connector.AtrelarEmpresaUsuario(empresa.id, usuario.id);

                return Ok(new
                {
                    code = 201,
                    success = true,
                    return_date = DateTime.Now,
                    data = new { }
                });

            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 400,
                    success = false,
                    return_date = DateTime.Now,
                    message = e.Message
                });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("criarBancos")]
        public IActionResult CriarBancos([FromBody] UserTokenViewModel user)
        {
            try
            {
                if (user == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Usuário não encontrado"
                    });
                }

                if (user.Id == 0 || !string.IsNullOrEmpty(user.AccessToken))
                {
                    return BadRequest(new
                    {
                        code = 400,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Usuário não encontrado"
                    });
                }

                var connector = new ZepelimADMConnector();
                var empresa = connector.CriarEmpresa(user.Id);
                if (empresa == null) throw new Exception("Erro ao criar ambiente");

                return Ok(new
                {
                    code = 201,
                    success = true,
                    return_date = DateTime.Now,
                    data = new { }
                });

            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 400,
                    success = false,
                    return_date = DateTime.Now,
                    message = e.Message
                });
            }
        }

        [HttpPost]
        [Route("FindById")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult FindById([FromBody] User user)
        {
            try
            {
                if (user.Id == 0)
                {
                    return NotFound(new
                    {
                        code = 404,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Usuário não encontrado"
                    });
                }

                var resOld = _userRepository.FindById(user.Id);

                if (resOld.Result == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Usuário não encontrado"
                    });
                }

                User userOld = resOld.Result;

                return Ok(new
                {
                    code = 200,
                    success = true,
                    return_date = DateTime.Now,
                    data = userOld
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 400,
                    success = false,
                    return_date = DateTime.Now,
                    message = e.Message
                });
            }
        }

        [HttpPost]
        [Route("ListAll")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult listAll()
        {
            try
            {
                var resOld = _userRepository.ListAllFans();

                if (resOld.Result == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Usuário não encontrado"
                    });
                }

                List<User> userOld = resOld.Result;

                return Ok(new
                {
                    code = 200,
                    success = true,
                    return_date = DateTime.Now,
                    data = userOld
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 400,
                    success = false,
                    return_date = DateTime.Now,
                    message = e.Message
                });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        [Route("RemoveInRange")]
        public IActionResult RemoveInRange([FromBody] List<int> userIds)
        {
            try
            {
                if (userIds.Count == 0)
                {
                    return NotFound(new
                    {
                        code = 404,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Usuário(s) não encontrado(s)"
                    });
                }

                _userRepository.RemoveInRange(userIds);

                return Ok(new
                {
                    code = 200,
                    return_date = DateTime.Now,
                    success = true,
                    data = ""
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 400,
                    success = false,
                    return_date = DateTime.Now,
                    message = e.Message
                });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        [Route("Remove")] 
        public IActionResult Remove([FromBody] OnlyIdViewModel toRemove)
        {
            try
            {
                var userOld = _userRepository.FindById(toRemove.Id);

                if (userOld.Result == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Usuário(s) não encontrado(s)"
                    });
                }

                _userRepository.RemoveById(toRemove.Id);

                return Ok(new
                {
                    code = 200,
                    return_date = DateTime.Now,
                    success = true,
                    data = ""
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 400,
                    success = false,
                    return_date = DateTime.Now,
                    message = e.Message
                });
            }
        }

        [HttpPost]
        [Route("SendRecoveryEmail")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult SendRecoveryEmail([FromBody] User userData)
        {
            if (userData == null)
            {
                return NotFound(new
                {
                    code = 404,
                    return_date = DateTime.Now,
                    success = false,
                    message = "E-mail não encontrado"
                });
            }
            
            // TO DO enviar e-mail com o link para a recuperação

            return Ok(new
            {
                code = 200,
                return_date = DateTime.Now,
                success = true,
                data = ""
            });
        }
    }
}
