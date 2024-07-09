using Article.Core.Common;
using Article.Infrastructure.ApplicationDbContext;
using Article.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

public class UnitOfWork : IUnitOfWork
{
    private readonly ArticleDbContext _context;
    private readonly IDbFactory _dbFactory;
    private Dictionary<Type, object> _repositories;

    public UnitOfWork(ArticleDbContext context, IDbFactory dbFactory)
    {
        _context = context;
        _dbFactory = dbFactory;
        _repositories = new Dictionary<Type, object>();
    }

    public IGenericRepositories<TEntity> Repository<TEntity>() where TEntity : class
    {
        if (_repositories == null)
        {
            _repositories = new Dictionary<Type, object>();
        }

        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type))
        {
            var repositoryInstance = new GenericRepository<TEntity>(_dbFactory);
            _repositories[type] = repositoryInstance;
        }

        return (IGenericRepositories<TEntity>)_repositories[type];
    }

    //public async Task<int> Commit()
    //{
    //    return await _context.SaveChangesAsync();
    //}

    public async Task<long> Commit()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Handle concurrency exceptions
            throw new Exception("Concurrency exception occurred.", ex);
        }
        catch (DbUpdateException ex)
        {
            // Handle specific errors for the DbContext
            throw new Exception("Database update exception occurred.", ex);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw new Exception("An error occurred while saving changes.", ex);
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
