using Article.Infrastructure.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;

namespace Article.Infrastructure.Common
{
    public class DbFactory : IDisposable, IDbFactory
    {
        private ArticleDbContext dbContext;
        private readonly DbContextOptions<ArticleDbContext> _dbContextOptions;

        public DbFactory(DbContextOptions<ArticleDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public ArticleDbContext Init()
        {
            return dbContext ?? (dbContext = new ArticleDbContext(_dbContextOptions));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                    dbContext = null;
                }
            }
        }
    }
}
