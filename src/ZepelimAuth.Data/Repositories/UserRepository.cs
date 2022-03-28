using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZAuth.Business.Interface;
using ZAuth.Database.Repository;
using ZepelimAuth.Business.Models;
using ZepelimAuth.Database;

namespace Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(Context context) : base(context) { }

        public virtual async Task<List<User>> ListAllFans()
        {
            return await DbSet
                .Where(usr => (usr.Removido == false) && (usr.Role == "FAN") )
               .AsNoTracking()
               .ToListAsync();
        }

        public virtual async Task<User> Get2(string Email, string Password)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(usr => usr.Email == Email && usr.Senha == Password);

            /*
            return users
                .FirstOrDefault(
                    x => string.Equals(x.Email, Email, StringComparison.CurrentCultureIgnoreCase)
                    && x.Password == Password
                );
            */
        }

        public virtual async Task<User> CheckIsUnique(string Cpf, string Email)
        {
            var query = DbSet.Where(usr => usr.Email == Email);

            if (Cpf.Length > 0)
            {
                query = query.Where(usr => usr.DocumentoUsuario == Cpf);
            }

            return await query.FirstOrDefaultAsync();
        }

        /*
        public virtual async Task<User> FindById(int userId)
        {
            return await DbSet.FirstOrDefaultAsync(usr => usr.Id == userId);
        }
        */

        /*
        public User Get(string Email, string Password)
        {
            var users = new List<User>
            {
                new User { Id = 1, Name = "William ADM", Email = "wpdcastro@gmail.com",  Password = "teste", Role = Role.ADM  },
                new User { Id = 2, Name = "William FAN", Email = "wpdcastro@gmail.com1", Password = "teste", Role = Role.FAN  }
            };

            // User user = _context.FindAsync(Email, Password);

            return users
                .FirstOrDefault(
                    x => string.Equals(x.Email, Email, StringComparison.CurrentCultureIgnoreCase)
                    && x.Password == Password
                );
        }
        */
    }
}
