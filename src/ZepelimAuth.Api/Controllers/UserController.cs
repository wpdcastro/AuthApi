using FanPush.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ZAuth.Business.Interface;
using ZepelimAuth.Business.Models;
using ZepelimAuth.Controllers.Utils;

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
        [Route("save")]
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

                if (user.Documento == null || user.Documento.Length != 14)
                {
                    return BadRequest(new
                    {
                        code = 400,
                        return_date = DateTime.Now,
                        success = false,
                        message = "Documento nulo ou inválido"
                    });
                }

                user.Role = "FAN";
                user.DataCadastro = DateTime.UtcNow;

                var resOld = _userRepository.CheckIsUnique(user.Documento, user.Email);

                if (user.Id > 0)
                {
                    resOld = _userRepository.FindById(user.Id);

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

                    userOld.Documento = user.Documento;
                    userOld.Name = user.Name;
                    userOld.Email = user.Email;
                    userOld.Role = user.Role;

                    if (user.Password.Trim().Length > 0)
                    {
                        userOld.Password = StringUtils.GerarHashMd5(user.Password);
                    }

                    var savedUser = _userRepository.Update(userOld);
                }
                else
                {
                    if (resOld.Result != null)
                    {
                        return BadRequest(new
                        {
                            code = 400,
                            return_date = DateTime.Now,
                            success = false,
                            message = "Cpf/E-mail já em uso"
                        });
                    }

                    user.Password = StringUtils.GerarHashMd5(user.Password);
                    var savedUser = _userRepository.Save(user);
                }

                user.Password = "";
                user.Id = 0;

                return Ok(new
                {
                    code = 200,
                    success = true,
                    return_date = DateTime.Now,
                    data = user
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
        [Route("saveAdm")]
        public IActionResult SaveAdm([FromBody] User user)
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

                user.Role = "ADM";
                user.DataCadastro = DateTime.UtcNow;

                var resOld = _userRepository.CheckIsUnique(user.Documento, user.Email);

                if (user.Id > 0)
                {
                    resOld = _userRepository.FindById(user.Id);

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

                    userOld.Name = user.Name;
                    userOld.Email = user.Email;
                    userOld.Role = user.Role;

                    if (user.Password.Trim().Length > 0)
                    {
                        userOld.Password = StringUtils.GerarHashMd5(user.Password);
                    }

                    var savedUser = _userRepository.Update(userOld);
                }
                else
                {
                    if (resOld.Result != null)
                    {
                        return BadRequest(new
                        {
                            code = 400,
                            success = false,
                            return_date = DateTime.Now,
                            message = "Já existe uma conta com este E-mail/CPF"
                        });
                    }
                        
                    user.Password = StringUtils.GerarHashMd5(user.Password);
                    var savedUser = _userRepository.Save(user);
                }

                user.Password = "";
                user.Id = 0;

                return Ok(new
                {
                    code = 200,
                    success = true,
                    return_date = DateTime.Now,
                    data = user
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
        [Route("findById")]
        [Authorize(Roles = "ADM,FAN")]
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
        [Route("listAll")]
        // [TokenAuthenticationFilter]
        [Authorize(Roles = "ADM")]
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
        [Route("removeInRange")]
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
        [Route("remove")] 
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
        [Route("sendRecoveryEmail")]
        [Authorize(Roles = "ADM,FAN")]
        public IActionResult SendRecovertEmail([FromBody] User userData)
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
