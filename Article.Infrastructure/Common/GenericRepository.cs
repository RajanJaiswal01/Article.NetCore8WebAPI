using Article.Core.Common;
using Article.Infrastructure.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Article.Infrastructure.Common
{
    public class GenericRepository<T>:IGenericRepositories<T> where T:class
    {

        private ArticleDbContext dbContext;
        private readonly IDbFactory _dbFactory;
        private readonly DbSet<T> _dbSet;

       
        protected ArticleDbContext DbContext
        {
            get
            {
                return dbContext ?? (dbContext = _dbFactory.Init());
            }
        }

        public GenericRepository(IDbFactory dbFactory)
        {
            _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
            _dbSet = DbContext.Set<T>();
        }


        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> GetById(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> GetById(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            //await DbContext.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _dbSet.Attach(entity);
            DbContext.Entry(entity).State = EntityState.Modified;
            //await DbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            _dbSet.Remove(entity);
            //await DbContext.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            var entity = await _dbSet.FindAsync(id);
            _dbSet.Remove(entity);
            //await DbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }
    }
}
