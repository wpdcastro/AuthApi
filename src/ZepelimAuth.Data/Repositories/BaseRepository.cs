using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZepelimAuth.Business.Interface;
using ZepelimAuth.Business.Models;
using ZepelimAuth.Database;

namespace ZAuth.Database.Repository
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : Entity, new()
    {
        protected readonly Context Db;
        protected readonly DbSet<TEntity> DbSet;

        protected BaseRepository(Context db)
        {
            Db = db;
            DbSet = db.Set<TEntity>();
        }
        public virtual async Task Save(TEntity entity)
        {
            var objAdd = DbSet.Add(entity);
            Db.SaveChanges();
            // await SaveChanges();
        }
        public virtual async Task<TEntity> Update(TEntity entity)
        {
            DbSet.Update(entity);
            Db.SaveChanges();
            // await SaveChanges();
            return entity;
        }
        public async Task<int> SaveChanges()
        {
            return await Db.SaveChangesAsync();
        }
        public async Task<TEntity> FindById(int id)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.Id == id);
        }
        public virtual async Task<List<TEntity>> FindAll()
        {
            return await DbSet.ToListAsync();
        }
        public virtual async Task RemoveById(int id)
        {
            DbSet.Remove(new TEntity { Id = id });
            await SaveChanges();
        }
        public virtual async Task RemoveInRange(List<int> ids)
        {
            foreach (var id in ids)
            {
                DbSet.Remove(DbSet.Find(id));
            }

            await SaveChanges();
        }
        public void Dispose()
        {
            Db?.Dispose();
        }
    }   
}
