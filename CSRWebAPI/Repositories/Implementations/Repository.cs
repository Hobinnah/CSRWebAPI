using CSRWebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {

        protected CSRDBContext _context;
        public Repository(CSRDBContext context)
        {
            this._context = context;
        }

        public Task Save() => _context.SaveChangesAsync();

        public async Task<bool> Any(Expression<Func<T, bool>> Predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(Predicate).AnyAsync();
        }

        public async Task<int> Count(Expression<Func<T, bool>> Predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(Predicate).CountAsync();
        }

        public async Task<T> Create(T Entity)
        {
            _context.Set<T>().Add(Entity);

            return Entity;
        }

        public async Task<T> Update(T Entity)
        {
            _context.Entry(Entity).State = EntityState.Modified;

            return Entity;
        }

        public async Task Delete(T Entity)
        {
            _context.Set<T>().Remove(Entity);
        }

        public async Task<T> FirstOrDefault()
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> Predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(Predicate).ToListAsync();
        }

        public IEnumerable<T> FindWhere(Expression<Func<T, bool>> Predicate)
        {
            return _context.Set<T>().AsNoTracking().Where(Predicate).ToList();
        }

        public async Task<T> Single(Expression<Func<T, bool>> Predicate)
        {
            return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(Predicate);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByID(int ID)
        {
            return await _context.Set<T>().FindAsync(ID);
        }

    }
}
