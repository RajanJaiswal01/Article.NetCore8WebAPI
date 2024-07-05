using Article.Core.Entities;
using Article.Core.IReposiories;
using Article.Infrastructure.ApplicationDbContext;
using Article.Infrastructure.Common;

namespace Article.Infrastructure.Repositories
{
    public class AuthorRepository:GenericRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(IDbFactory dbFactory):base(dbFactory)
        {
            
        }
    }
}
