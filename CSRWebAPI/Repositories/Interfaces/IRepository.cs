using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> Predicate);
        Task<T> GetByID(int ID);
        Task<bool> Any(Expression<Func<T, bool>> Predicate);
        Task<T> FirstOrDefault();
        Task Save();
        Task<T> Create(T Entity);
        Task<T> Update(T Entity);
        Task Delete(T Entity);
        Task<int> Count(Expression<Func<T, bool>> Predicate);
        Task<T> Single(Expression<Func<T, bool>> Predicate);
        IEnumerable<T> FindWhere(Expression<Func<T, bool>> Predicate);
    }
}
