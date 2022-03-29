using System;
using System.IO;
using System.Linq;
using System.Net;
using ZepelimAuth.Business.Models;
using System.Text.Json;
using ZepelimADM.Models;

namespace ZepelimADM.Connections
{
    public class ZepelimADMConnector
    {
        protected string _link = "http://localhost:8090/api/v1/";
        protected string _loggedUser = "";
        public ZepelimADMConnector()
        {
            // _loggedUser = GetToken();
            _loggedUser = "";
        }

        public string GetToken()
        {
            var link = _link + "login";
            var request = HttpWebRequest.Create(@"" + link);
            request.ContentType = "application/json";
            request.Method = "GET";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var res = response.GetResponseStream();
                    throw new Exception("Erro ao conectar com o Zepelim ADM");
                };

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        return "Erro Empty Response";
                    }
                    else
                    {
                        dynamic jsonString = JsonSerializer.Serialize(content);
                        return "";

                        /*
                        dynamic jsonNE = JsonConvert.DeserializeObject(content);
                        JObject j1NE   = JObject.Parse(retornoEsp);
                        JObject j2NE = ((Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(j1NE.First.First.ToString().Replace("}", ",") + j1NE.Last.ToString() + "}"));
                        User UserAPI = JsonConvert.DeserializeObject<User>(j2NE.ToString());
                        */
                    }
                }
            }
        }

        public ObjectADM AlterarToken(int userId, string token, string refreshToken)
        {
            try
            { 
                string link = _link + "login/alterarToken";
                var request = HttpWebRequest.Create(@"" + link);
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "{\"UserId\":\"" + userId + "\", \"AccessToken\":\"" + token + "\", \"RefreshToken\":\"" + refreshToken + "\"}";

                    streamWriter.Write(json);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var res = response.GetResponseStream();
                        throw new Exception("Erro ao conectar com o Zepelim!");
                    };

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();

                        if (string.IsNullOrWhiteSpace(content))
                        {
                            throw new Exception("Erro ao conectar com o Ideris!");
                        }
                        else
                        {
                            var retorno = JsonSerializer.Deserialize<EmpresaReturn>(content);
                            return retorno.data;
                        }
                    }
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        throw new Exception(text);
                    }
                }

                throw new Exception("Erro ao tentar acessar API", e);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao tentar acessar API", ex);
            }
        }

        public UserLoginADM Logar(string Login, string Senha)
        {
            try
            {
                string link = _link + "login/logar";
                var request = HttpWebRequest.Create(@"" + link);
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "{\"Email\":\"" + Login + "\", \"Senha\":\"" + Senha + "\"}";

                    streamWriter.Write(json);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var res = response.GetResponseStream();
                        throw new Exception("Erro ao conectar com o Zepelim!");
                    };

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();

                        if (string.IsNullOrWhiteSpace(content))
                        {
                            throw new Exception("Erro ao conectar com o Ideris!");
                        }
                        else
                        {
                            var userLogado = JsonSerializer.Deserialize<LoginReturn>(content);
                            return userLogado.data;
                        }
                    }
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        throw new Exception(text);
                    }
                }

                throw new Exception("Erro ao tentar acessar API", e);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao tentar acessar API", ex);
            }
        }

        public ObjectADM CriarUsuario(User user)
        {
            try
            {
                string link = _link + "user/save";

                var request = HttpWebRequest.Create(@"" + link);
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "{\"Nome\":\"" + user.Nome + "\", \"Documento\":\"" + user.DocumentoUsuario + "\",";
                    json += "\"Email\":\"" + user.Email + "\", \"Role\": \"" + user.Role + "\", \"Senha\":\"" + user.Senha + "\"}";

                    streamWriter.Write(json);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var res = response.GetResponseStream();
                        throw new Exception("Erro ao conectar com o Zepelim!");
                    };

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();

                        if (string.IsNullOrWhiteSpace(content))
                        {
                            throw new Exception("Erro ao conectar com o Ideris!");
                        }
                        else
                        {
                            var empresaSalva = JsonSerializer.Deserialize<EmpresaReturn>(content);
                            return empresaSalva.data;
                            // return content;
                        }
                    }
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        throw new Exception(text);
                    }
                }

                throw new Exception("Erro ao tentar acessar API", e);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao tentar acessar API", ex);
            }
        }

        public UserADM Login(string login, string password)
        {
            try
            {
                string link = _link + "FindUser?Login=" + login + "&Password=" + password;

                var request = HttpWebRequest.Create(@"" + link);
                request.ContentType = "application/json";
                request.Method = "POST";
                request.PreAuthenticate = true;
                request.Headers.Add("Authorization", _loggedUser);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var res = response.GetResponseStream();
                        throw new Exception("Erro ao conectar com o Zepelim!");
                    };

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();

                        if (string.IsNullOrWhiteSpace(content))
                        {
                            return null;
                        }
                        else
                        {
                            dynamic jsonString = JsonSerializer.Serialize(content);
                            return new UserADM();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao tentar acessar API", ex);
            }
        }

        public ObjectADM CriarEmpresa(User user)
        {
            try
            {
                string link = _link + "empresa/save";
                var request = HttpWebRequest.Create(@"" + link);
                request.ContentType = "application/json";
                request.Method = "POST";
                // request.PreAuthenticate = true;
                // request.Headers.Add("Authorization", _loggedUser);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "{\"Descricao\":\"" + user.RazaoSocial + "\", \"Documento\":\"" + user.DocumentoEmpresa + "\"}";
                    streamWriter.Write(json);
                }

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var res = response.GetResponseStream();
                    throw new Exception("Erro ao conectar com o Zepelim!");
                };

                StreamReader reader = new StreamReader(response.GetResponseStream());

                var content = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(content))
                {
                    throw new Exception("Erro ao conectar com o Ideris!");
                }
                else
                {
                    var empresaSalva = JsonSerializer.Deserialize<EmpresaReturn>(content);

                    if (empresaSalva.message != null && empresaSalva.message.Length > 0) throw new Exception(empresaSalva.message);
                    return empresaSalva.data;
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        throw new Exception(text);
                    }
                }

                throw new Exception("Erro ao tentar acessar API", e);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao tentar acessar API", ex);
            };
        }

        public ObjectADM AtrelarEmpresaProduto(int empresaId, int produtoId)
        {
            try
            {
                string link = _link + "empresa/saveempprd";

                var request = HttpWebRequest.Create(@"" + link);
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "{\"EmpresaId\":" + empresaId.ToString() + ", \"ProdutoId\":" + produtoId.ToString() + ", \"ConnectionString\": \"\"}";

                    streamWriter.Write(json);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var res = response.GetResponseStream();
                        throw new Exception("Erro ao conectar com o Zepelim!");
                    };

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();

                        if (string.IsNullOrWhiteSpace(content))
                        {
                            throw new Exception("Erro ao conectar com o Zepelim!");
                        }
                        else
                        {
                            var empresaSalva = JsonSerializer.Deserialize<EmpresaReturn>(content);
                            return empresaSalva.data;
                        }
                    }
                }

                return null;
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        throw new Exception(text);
                    }
                }

                throw new Exception("Erro ao tentar acessar API", e);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public ObjectADM AtrelarEmpresaUsuario(int empresaId, int usuarioId)
        {
            try
            {
                string link = _link + "empresa/saveempuser";

                var request = HttpWebRequest.Create(@"" + link);
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "{\"EmpresaId\":\"" + empresaId + "\", \"UsuarioId\":\"" + usuarioId + "\"}";
                    streamWriter.Write(json);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var res = response.GetResponseStream();
                        throw new Exception("Erro ao conectar com o Zepelim!");
                    };

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();

                        if (string.IsNullOrWhiteSpace(content))
                        {
                            throw new Exception("Erro ao conectar com o Zepelim!");
                        }
                        else
                        {
                            var empresaSalva = JsonSerializer.Deserialize<EmpresaReturn>(content);
                            return empresaSalva.data;
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        
        public void CriaBancos(UserADM usuarioLogado)
        {

        }
    }
}
