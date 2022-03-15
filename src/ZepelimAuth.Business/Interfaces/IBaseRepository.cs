using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZepelimAuth.Business.Models;

namespace ZepelimAuth.Business.Interface
{
    public interface IBaseRepository<TEntity> : IDisposable where TEntity : Entity
    {
        Task<TEntity> FindById(int id);
        Task Save(TEntity entity);
        Task<TEntity> Update(TEntity entity);
        Task<int> SaveChanges();
        Task<List<TEntity>> FindAll();
        Task RemoveInRange(List<int> ids);
        Task RemoveById(int id);
    }
}
