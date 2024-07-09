using Article.Core.Entities;
using Article.Core.IReposiories;
using Article.Infrastructure.ApplicationDbContext;
using Article.Infrastructure.Common;

namespace Article.Infrastructure.Repositories
{
    public class BlogRepository:GenericRepository<Blog>,IBlogRepository
    {
        public BlogRepository(ArticleDbContext dbContext) : base(dbContext)
        {
            
        }
    }
}
