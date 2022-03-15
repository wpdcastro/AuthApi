using System.Collections.Generic;
using System.Threading.Tasks;
using ZepelimAuth.Business.Interface;
using ZepelimAuth.Business.Models;

namespace ZAuth.Business.Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> Get2(string Email, string Password);
        Task<User> CheckIsUnique(string Cpf, string Email);
        Task<List<User>> ListAllFans();
    }
}
