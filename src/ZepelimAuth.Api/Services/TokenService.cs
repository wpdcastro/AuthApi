using FanPush.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ZepelimADM.Models;
using ZepelimAuth.Business.Models;

namespace FanPush.Services
{
    public static class TokenService
    {
        public static string GenerateToken(UserLoginADM user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(Settings.Secret);

            if (user == null) throw new Exception("Usuário não encontrado");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.email),
                    new Claim(ClaimTypes.Role, user.role)
                }),
                Expires = DateTime.Now.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            tokenDescriptor.Subject.AddClaim(new Claim(type: "UsuarioId", value: user.id.ToString()));
            tokenDescriptor.Subject.AddClaim(new Claim(type: "EmpresaAtiva", value: user.empresaAtivaId.ToString()));

            foreach (ProductoADM produto in user.produtos)
            {
                tokenDescriptor.Subject.AddClaim(
                    new Claim(type: produto.descricao.ToUpper(), value: (produto.connectionString.Length > 0) ? "true" : "false")
                );
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string GenerateToken(UserADM user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(Settings.Secret);

            if (user == null) throw new Exception("Usuário não encontrado");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(type: "isPOS", value: user.POS.ToString().ToLower()),
                    new Claim(type: "isHUB", value: user.HUB.ToString().ToLower()),
                    new Claim(type: "isLOG", value: user.LOG.ToString().ToLower())
                }),
                Expires = DateTime.Now.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            foreach (var produto in user.Produtos)
            {
                tokenDescriptor.Subject.AddClaim(
                    new Claim(type: produto.Descricao.ToUpper(), value: produto.ConnectionString)
                );
            }

            foreach (var produto in user.EmpresasAtivas)
            {
                /*
                tokenDescriptor.Subject.AddClaim(
                    new Claim(type: produto.Descricao.ToUpper(), value: produto.ConnectionString)
                );
                */
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static string GenerateToken(IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(Settings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                throw new SecurityTokenException("Invalid Token");
            }

            return principal;
        }
        private static List<(string, string)> _refreshTokens = new();
        public static void SaveRefreshToken(string Email, string refreshToken)
        {
            _refreshTokens.Add(new(Email, refreshToken));
        }
        public static string GetRefreshToken(string Email)
        {
            return _refreshTokens.FirstOrDefault(x => x.Item1 == Email).Item2;
        }
        public static void DeleteRefreshToken(string Email, string refreshToken)
        {
            var item = _refreshTokens.FirstOrDefault(x => x.Item1 == Email && x.Item2 == refreshToken);
            _refreshTokens.Remove(item);
        }
    }
}
