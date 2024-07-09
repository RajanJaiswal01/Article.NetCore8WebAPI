using Article.Core.Common;
using Article.Infrastructure.ApplicationDbContext;
using Article.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

public class UnitOfWork : IUnitOfWork
{
    private readonly ArticleDbContext _context;
    private readonly IDbFactory _dbFactory;
    private Dictionary<Type, object> _repositories;

    public UnitOfWork(ArticleDbContext context)
    {
        _context = context;
    }



    public async Task<long> CommitAsync()
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
    
    
    public void Commit()
    {
        try
        {
            _context.SaveChangesAsync();
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
