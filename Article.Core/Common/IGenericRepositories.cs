using System.Linq.Expressions;

namespace Article.Core.Common
{
    public interface IGenericRepositories<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate);
        Task<T> GetById(int id);
        Task<T> GetById(long id);
        Task<T> GetById(Expression<Func<T, bool>> predicate);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
        Task Delete(long id);
    }
}
