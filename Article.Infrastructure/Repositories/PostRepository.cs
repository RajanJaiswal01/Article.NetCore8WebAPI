using Article.Core.Entities;
using Article.Core.IReposiories;
using Article.Infrastructure.Common;

namespace Article.Infrastructure.Repositories
{
    public class PostRepository:GenericRepository<Post>,IPostRepository
    {
        public PostRepository(IDbFactory dbFactory):base(dbFactory)
        {
            
        }
    }
}
